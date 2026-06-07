# MetricsHub

An internal webhook aggregation API that receives events from Pulse and Sentry, normalises them into a unified model, persists them to PostgreSQL, and exposes a filterable query endpoint.

---

## Running the solution

### Prerequisites

- Docker Desktop running

### 1. Clone and navigate to the solution folder

```bash
git clone <repo-url>
cd MetricsHub/MetricsHub
```

### 2. Start the stack

```bash
docker compose up --build
```

Docker Compose will:

1. Start a `postgres:16-alpine` container and wait until it passes its health check
2. Build the API image from source
3. Start the API, which runs EF Core migrations automatically on startup

Wait until you see a log line similar to:

```
api  | info: Microsoft.Hosting.Lifetime[14]
api  |       Now listening on: http://[::]:8080
```

The first build downloads base images and restores NuGet packages — subsequent builds are faster.

### 3. Verify the API is up

Open a browser or run:

```bash
curl http://localhost:8080/health
```

Expected response: `Healthy`

### 4. Send sample requests

Two options are provided — use whichever suits your tooling.

#### Option A — `.http` file (Visual Studio / VS Code)

Open `src/MetricsHub.Api/MetricsHub.Api.http` in Visual Studio or VS Code (with the [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) extension).

Execute the requests in order:

1. **Pulse 1** — healthy worker, production
2. **Pulse 2** — high CPU worker, staging
3. **Pulse 3** — low activity worker, production
4. **Sentry 1** — high error rate alert (correlates with Pulse 1)
5. **Sentry 2** — memory threshold breach (correlates with Pulse 2)
6. **Sentry 3** — latency spike, no correlation
7. **All events** — confirm all 6 events are stored
8. **Filter by source — pulse only** — returns 3 events
9. **Filter by source — sentry only** — returns 3 events
10. **Filter by exact node name** — returns events for `worker-us-east-01`
11. **Filter by time range** — narrows results to a specific window
12. **Filter by source and time range** — combined filter
13. **Paginated page 1** — first 2 results
14. **Paginated page 2** — next 2 results

#### Option B — Postman collection

1. Open Postman and click **Import** (top-left).
2. Select `MetricsHub.postman_collection.json` from the repository root.
3. The collection imports with a `baseUrl` variable already set to `http://localhost:8080`.
4. Open the **Pulse Events** folder and send requests 1–3 in order.
5. Open the **Sentry Events** folder and send requests 1–3 in order.
6. Open the **Event Queries** folder and run any combination of the 8 query requests.
7. Use the **Health check** folder to verify the API is still healthy at any point.

The collection is organised into four folders:

| Folder | Requests |
|--------|----------|
| Pulse Events | Pulse 1, 2, 3 |
| Sentry Events | Sentry 1, 2, 3 |
| Event Queries | All events, filter by source ×2, filter by node, filter by time range, combined filter, paginated ×2 |
| Health check | Health |

### 5. Stop

```bash
docker compose down
```

Add `-v` to also remove the named Postgres volume and start fresh.

---

## Port mappings

| Service  | Host port | Container port | URL                        |
|----------|-----------|----------------|----------------------------|
| API      | 8080      | 8080           | http://localhost:8080      |
| Postgres | 5432      | 5432           | localhost:5432             |

### Endpoints

| Method | Path       | Description                                      |
|--------|------------|--------------------------------------------------|
| POST   | /webhooks  | Ingest a Pulse or Sentry event                   |
| GET    | /events    | Query normalised events with filtering/pagination |
| GET    | /health    | Liveness + database health check                 |

### Ingestion

Send the raw source payload with an `X-Source` header identifying the origin:

```bash
curl -X POST http://localhost:8080/webhooks \
  -H "Content-Type: application/json" \
  -H "X-Source: pulse" \
  -d '{"pulse_id":"p-001","ts":"2026-06-07T10:00:00Z","node":"worker-01","region":"us-east-1","metrics":{"cpu_pct":42.1,"mem_mb":2048,"active_conns":87,"status":"healthy"},"tags":["production"]}'
```

| Condition                     | Status |
|-------------------------------|--------|
| Success                       | 201    |
| Missing / unknown `X-Source`  | 400    |
| Invalid or missing fields     | 422    |

### Query

```
GET /events?source=pulse&node=worker-01&from=2026-06-07T00:00:00Z&to=2026-06-07T23:59:59Z&page=1&pageSize=20
```

All parameters are optional:

| Parameter  | Type        | Description                              |
|------------|-------------|------------------------------------------|
| `source`   | string      | `pulse` or `sentry` (case-insensitive)   |
| `node`     | string      | Exact match on node name                 |
| `from`     | ISO-8601    | Lower bound on `occurredAt`              |
| `to`       | ISO-8601    | Upper bound on `occurredAt`              |
| `page`     | int         | Page number, 1-based (default: 1)        |
| `pageSize` | int         | Items per page, max 100 (default: 20)    |

Response envelope:

```json
{
  "items": [...],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20
}
```

A `.http` file with ready-to-run sample requests is at `src/MetricsHub.Api/MetricsHub.Api.http`.

---

## Running the tests

### Unit tests (no Docker required)

```bash
dotnet test tests/MetricsHub.Unit.Tests
```

### Integration tests (Docker required)

```bash
dotnet test tests/MetricsHub.Integration.Tests
```

Integration tests use Testcontainers to spin up a dedicated PostgreSQL container per test class. The application is hosted in-process via `WebApplicationFactory`.

---

## Design decisions

### Single ingestion endpoint with `X-Source` routing

A single `POST /webhooks` endpoint keeps the public surface minimal and mirrors how a real webhook broker would work — the consumer declares the source, the server normalises it. An alternative would be source-specific routes (`/webhooks/pulse`, `/webhooks/sentry`), but that leaks the source topology into the URL and requires routing logic elsewhere.

### Strategy pattern for normalisation

Each source system has its own `IEventNormalizer` implementation (`PulseNormalizer`, `SentryNormalizer`). `NormalizerStrategy` resolves the correct one by source name at runtime from a dictionary built at startup. Adding a new source requires only a new class and a DI registration — no changes to existing code. All normaliser implementations are registered as `IEventNormalizer` and collected automatically via `IEnumerable<IEventNormalizer>` injection.

### Unified `NormalizedEvent` model with `jsonb` metrics

Rather than a rigid schema for every possible source metric, source-specific fields are stored in a `jsonb` column. This keeps the entity stable as sources evolve, and PostgreSQL's `jsonb` type allows indexing and querying those fields later without a schema migration. The `RawPayload` column preserves the original bytes verbatim for auditability and reprocessing.

### Clean Architecture with four projects

| Project               | Responsibility                                      |
|-----------------------|-----------------------------------------------------|
| `MetricsHub.Domain`   | `NormalizedEvent` entity and `EventSource` enum     |
| `MetricsHub.Application` | Normalisation logic, services, DTOs             |
| `MetricsHub.Infrastructure` | EF Core `DbContext` and migrations           |
| `MetricsHub.Api`      | Controllers, middleware, DI wiring                  |

Services use `MetricsHubDbContext` directly — no repository layer. The abstraction a repository would add is not justified here given the single entity and the fact that EF Core's `DbContext` is already a unit of work.

### Global exception handler middleware

`GlobalExceptionHandler` (implementing `IExceptionHandler`, .NET 8) centralises error-to-HTTP-status mapping in one place. Controllers contain no `try/catch` blocks. The mapping is:

- `UnknownSourceException` → 400
- `InvalidPayloadException` → 422
- `OperationCanceledException` → swallowed silently (client disconnect)
- Anything else → 500 with a structured log entry

`InvalidPayloadException` and `UnknownSourceException` both derive from `MetricsHubException`, so service-level catch blocks can target domain errors specifically without catching framework or infrastructure exceptions.

### Correlation

`CorrelationId` is stored on every event. Sentry events populate it from the `correlation_id` field in the payload (null if absent or empty). Pulse events always store null — consumers can match a Sentry alert to its corresponding Pulse heartbeat by querying both sources and joining on the Sentry event's `correlationId`. No server-side join endpoint was implemented, but the data model supports one.

### EF Core migrations at startup

`MigrateAsync()` runs in `Program.cs` before the application starts accepting requests. This keeps the schema in sync automatically without a separate migration step, which is appropriate for a single-instance deployment. Docker Compose ensures Postgres is healthy before the API starts via `depends_on: condition: service_healthy`.

---

## What I would do differently with more time or in a production context



---

## Known limitations and shortcuts


