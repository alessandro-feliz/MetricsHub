# MetricsHub — Implementation Plan

## Problem Summary

Build an internal metrics-aggregation API that:
- Receives webhook events from two source systems (Pulse and Sentry) at a single endpoint
- Normalizes both formats into a unified internal model
- Persists normalized events to a relational database
- Exposes a query endpoint with filtering by time range, source, and resource name
- Handles correlation between Sentry alerts and Pulse heartbeats
- Runs end-to-end via `docker compose up` with no local tooling required

---

## Clarifications & Decisions

| Question | Decision |
|---|---|
| Persistence | PostgreSQL + EF Core (compose service) |
| Webhook routing | Single `POST /webhooks` endpoint, source determined by `X-Source` header |
| Correlation | Implement: store `correlation_id` on the unified model and expose it in query responses |
| Test scope | Unit tests (normalization) + integration tests (WebApplicationFactory over HTTP) |
| Layering | 4-project Clean Architecture; services use `DbContext` directly — no repository layer |

---

## Architecture

### Solution Layout

```
MetricsHub/
├── MetricsHub.slnx
├── docker-compose.yml
├── src/
│   ├── MetricsHub.Domain/            # Entities only — no framework dependencies
│   ├── MetricsHub.Application/       # Services, normalizers, payload models
│   ├── MetricsHub.Infrastructure/    # EF Core DbContext + migrations
│   └── MetricsHub.Api/               # Controllers, DI wiring, middleware
└── tests/
    ├── MetricsHub.Unit.Tests/        # Normalization logic, pure C# — no framework deps
    └── MetricsHub.Integration.Tests/ # WebApplicationFactory HTTP tests — requires Postgres
```

### Dependency Rules

```
Api  →  Application  →  Domain
Infrastructure  →  Application (DbContext registered; Application references it for injection)
Api  →  Infrastructure (DI registration only)
```

### Project Detail

Code is organized by **feature** within each project. Features are `Webhooks` (ingestion) and `Events` (query).

#### `MetricsHub.Domain`
```
NormalizedEvent.cs    # Single shared entity — no feature subfolder (one entity)
```
No EF attributes, no NuGet dependencies beyond the BCL.

#### `MetricsHub.Application`
```
Services/
  Webhooks/
    WebhookIngestionService.cs   # Orchestrates: resolve strategy → normalize → persist
  Events/
    EventQuery.cs                # Query parameters (from, to, source, resource, page, pageSize)
    PagedResult.cs               # Generic paged response envelope
    EventQueryService.cs         # Filtering + pagination over DbContext
Normalization/
  IEventNormalizer.cs            # Strategy interface: SourceName + Normalize(payload)
  NormalizerStrategy.cs          # Context: resolves and invokes the correct strategy by source name
  Pulse/
    PulsePayload.cs              # Deserialization + validation model
    PulseNormalizer.cs           # Concrete strategy: Pulse JSON → NormalizedEvent
  Sentry/
    SentryPayload.cs
    SentryNormalizer.cs          # Concrete strategy: Sentry JSON → NormalizedEvent
```

Services receive `MetricsHubDbContext` via constructor injection and use it directly.

#### `MetricsHub.Infrastructure`
```
MetricsHubDbContext.cs         # EF Core DbContext; DbSet<NormalizedEvent>
Migrations/                    # EF-generated migration files
```

#### `MetricsHub.Api`
```
Webhooks/
  WebhooksController.cs        # POST /webhooks  →  WebhookIngestionService
Events/
  EventsController.cs          # GET /events     →  EventQueryService
Program.cs                     # DI wiring, middleware, MigrateAsync on startup
```

#### `MetricsHub.Unit.Tests`
Dependencies: xUnit, FluentAssertions. No EF Core, no HTTP stack.
```
Webhooks/
  PulseNormalizerTests.cs
  SentryNormalizerTests.cs
```

#### `MetricsHub.Integration.Tests`
Dependencies: xUnit, WebApplicationFactory, Testcontainers.PostgreSql.
```
Webhooks/
  WebhooksEndpointTests.cs
Events/
  EventsEndpointTests.cs
```

---

### Unified Event Model (`NormalizedEvent`)

```csharp
public enum EventSource { Pulse, Sentry }

public class NormalizedEvent
{
    public Guid Id { get; set; }
    public EventSource Source { get; set; }       // stored as string via EF value converter
    public string SourceEventId { get; set; }     // pulse_id or alert_id
    public DateTimeOffset OccurredAt { get; set; } // ts or fired_at
    public DateTimeOffset IngestedAt { get; set; } // server receipt time
    public string Node { get; set; }              // node or resource.name
    public string Region { get; set; }            // region or resource.group
    public string Environment { get; set; }       // from tags or resource.environment
    public string? CorrelationId { get; set; }    // Sentry correlation_id; Pulse self-references pulse_id
    public string RawPayload { get; set; }        // original JSON payload as received
    public JsonDocument Metrics { get; set; }     // source-specific fields as a JSON column
}
```

`Metrics` is a `jsonb` column in Postgres — each normalizer writes the source-specific fields it cares about into it without the entity needing to know what those fields are. `RawPayload` preserves the original bytes verbatim for auditability. `Source` is stored as a string via an EF value converter so migrations stay readable.

### Service Contracts

```csharp
// Application/Services/WebhookIngestionService.cs
public class WebhookIngestionService(MetricsHubDbContext db, NormalizerRegistry normalizers)
{
    public Task<Guid> IngestAsync(string source, JsonDocument payload, CancellationToken ct);
}

// Application/Services/EventQueryService.cs
public class EventQueryService(MetricsHubDbContext db)
{
    public Task<PagedResult<NormalizedEvent>> QueryAsync(EventQuery query, CancellationToken ct);
}
```

### Normalization — Strategy Pattern

```csharp
// Strategy interface (Application/Webhooks/IEventNormalizer.cs)
public interface IEventNormalizer
{
    string SourceName { get; }   // "pulse" | "sentry"
    NormalizedEvent Normalize(JsonDocument payload);
}

// Concrete strategies
// Application/Webhooks/Pulse/PulseNormalizer.cs
public class PulseNormalizer : IEventNormalizer { ... }

// Application/Webhooks/Sentry/SentryNormalizer.cs
public class SentryNormalizer : IEventNormalizer { ... }

// Context — selects and invokes the strategy (Application/Webhooks/NormalizerStrategy.cs)
public class NormalizerStrategy
{
    private readonly IReadOnlyDictionary<string, IEventNormalizer> _normalizers;

    public NormalizerStrategy(IEnumerable<IEventNormalizer> normalizers) =>
        _normalizers = normalizers.ToDictionary(n => n.SourceName, StringComparer.OrdinalIgnoreCase);

    public NormalizedEvent Normalize(string sourceName, JsonDocument payload) =>
        _normalizers.TryGetValue(sourceName, out var normalizer)
            ? normalizer.Normalize(payload)
            : throw new UnknownSourceException(sourceName);
}
```

All `IEventNormalizer` implementations are registered in DI; `NormalizerStrategy` receives them via `IEnumerable<IEventNormalizer>` — adding a new source requires only a new class, no changes to existing code. Unknown source → `UnknownSourceException` → controller returns 400.

### Endpoint Summary

| Method | Path | Description |
|---|---|---|
| `POST` | `/webhooks` | Ingest Pulse or Sentry event; `X-Source` header required |
| `GET` | `/events` | Query normalized events |
| `GET` | `/health` | Liveness + DB health check |

#### `GET /events` Query Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `from` | ISO-8601 | — | Lower bound on `OccurredAt` |
| `to` | ISO-8601 | — | Upper bound on `OccurredAt` |
| `source` | string | — | `pulse` or `sentry` |
| `resource` | string | — | Substring match on `Node` |
| `page` | int | 1 | Page number (1-based) |
| `pageSize` | int | 20 | Items per page (max 100) |

Response envelope: `{ totalCount, page, pageSize, items }`.

### Docker Compose Services

```
api        — MetricsHub.Api on port 8080
postgres   — postgres:16-alpine, named volume, healthcheck
```

API waits via `depends_on: condition: service_healthy`. `MigrateAsync()` runs on startup.

---

## Correlation Design

`CorrelationId` is stored on every `NormalizedEvent`:
- **Pulse**: set to its own `pulse_id` — consumers can match it to Sentry alerts.
- **Sentry**: set to `correlation_id` if non-null/non-empty, otherwise `null`.

Consumers query `GET /events?source=sentry` and use `correlationId` to fetch the matching Pulse event. No server-side join endpoint in scope, but the data model supports one.

A Sentry event may reference a Pulse event not yet received. The API stores it as-is; correlation is resolved lazily by the consumer.

---

## "Could Have" Items (Documented, Not Implemented)

### Idempotency
Key on `(Source, SourceEventId)`. Add a unique index; catch the constraint exception in `WebhookIngestionService` and return the existing event `Id` with `200 OK`. Keeps webhook senders stateless.

### Rate Limiting
ASP.NET Core built-in `RateLimiter` middleware — fixed-window limiter (e.g. 100 req/min per IP) on `/webhooks`. For multi-instance deployments, back the limiter with Redis.

### Health Check
Already planned as a must-have via `Microsoft.Extensions.Diagnostics.HealthChecks` with an EF Core DB probe.

---

## Iterations

### Iteration 1 — Foundation
- Create `MetricsHub.Domain`, `MetricsHub.Application`, `MetricsHub.Infrastructure`, `MetricsHub.Unit.Tests`, `MetricsHub.Integration.Tests` projects; add all to solution
- Add `NormalizedEvent` entity to Domain
- Wire EF Core + Npgsql in Infrastructure (`MetricsHubDbContext` with `DbSet<NormalizedEvent>`)
- Add `docker-compose.yml` (`api` + `postgres` services)
- `Program.cs`: register DbContext, run `MigrateAsync()` on startup, map `/health` endpoint
- Generate initial EF migration

**Exit criteria:** `docker compose up` starts both containers; `/health` returns 200; Postgres schema is created.

### Iteration 2 — Normalization
- Add `PulsePayload` and `SentryPayload` models with validation attributes in Application
- Implement `PulseNormalizer`, `SentryNormalizer`, `NormalizerRegistry`
- Unit tests: valid payloads produce correct `NormalizedEvent`; missing required fields throw

**Exit criteria:** `dotnet test tests/MetricsHub.Unit.Tests` is green.

### Iteration 3 — Webhook Endpoint
- Implement `WebhookIngestionService` in Application
- Implement `WebhooksController` (`POST /webhooks`) in Api — delegates entirely to the service
- Validation: missing/unknown `X-Source` → 400; malformed payload → 422 with field errors
- Integration tests in `MetricsHub.Integration.Tests`: happy path (Pulse + Sentry); bad header; malformed JSON; missing required fields

**Exit criteria:** `dotnet test tests/MetricsHub.Integration.Tests` passes; `curl` to running container round-trips successfully.

### Iteration 4 — Query Endpoint
- Implement `EventQuery` and `PagedResult` in Application
- Implement `EventQueryService` with `IQueryable` filtering chain
- Implement `EventsController` (`GET /events`) in Api
- Integration tests in `MetricsHub.Integration.Tests`: unfiltered returns all; each filter narrows correctly; pagination metadata is accurate

**Exit criteria:** `dotnet test tests/MetricsHub.Integration.Tests` passes; persisted events are retrievable and filterable.

### Iteration 5 — Correlation & Polish
- Confirm `CorrelationId` is populated and returned correctly in query responses
- Remove `WeatherForecast` scaffold files
- Add structured `ILogger<T>` logging in services (event received, normalization failure, DB error)
- Add `/health` EF Core DB health check
- Final `docker-compose.yml` tuning (env vars, restart policy, Dockerfile build context)

**Exit criteria:** `docker compose up --build` from a clean clone with no local SDK; all tests pass; logs are structured.

---

## Out of Scope
- Authentication / API keys
- Event schema versioning
- Outbound notifications or streaming
- Multi-tenant isolation
