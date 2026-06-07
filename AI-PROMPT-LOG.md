# AI Prompt Log

## [2026-06-07 17:00] Add Postman collection and update README step 4

**Prompt:** Add a Postman collection JSON file users can import alongside the existing .http file; update the Send sample requests section to include both options with import steps.
**Files affected:** MetricsHub.postman_collection.json, README.md

## [2026-06-07 16:35] Expand Running the solution chapter with step-by-step instructions

**Prompt:** Update the Running the solution section with numbered steps: clone, navigate, docker compose up, verify health, execute .http requests in order, stop.
**Files affected:** README.md

## [2026-06-07 16:30] Write README.md

**Prompt:** Create a README covering how to build and run, port mappings, API usage, design decisions with reasoning, what would be done differently in production, and known limitations.
**Files affected:** README.md

## [2026-06-07 16:20] Create HTTP requests file with pulse, sentry, and query examples

**Prompt:** Create a .http file with 3 Pulse webhook requests, 3 Sentry webhook requests, and event query requests covering all filter combinations.
**Files affected:** src/MetricsHub.Api/MetricsHub.Api.http

## [2026-06-07 16:10] Configure JsonStringEnumConverter globally

**Prompt:** Serialize enums as strings in all JSON responses by adding JsonStringEnumConverter to AddControllers().AddJsonOptions() in Program.cs.
**Files affected:** src/MetricsHub.Api/Program.cs

## [2026-06-07 16:05] Change node filter to equality and fix pagination tests

**Prompt:** Fix failing Get_FilterBySourcePulse test; change EventQueryService node filter from Contains to == (exact match); update pagination tests which relied on prefix-based Contains matching to use the same node name for all seeded events.
**Files affected:** src/MetricsHub.Application/Services/Events/EventQueryService.cs, tests/MetricsHub.Integration.Tests/Events/EventsControllerTests.cs

## [2026-06-07 15:55] Fix integration test isolation after Resource→Node rename

**Prompt:** Fix failing integration tests caused by: (1) URL parameter still using ?resource= after EventQuery.Resource was renamed to Node — updated all test URLs to ?node=; (2) prefix collision where ?node=node-page also matched node-page2 events — renamed prefixes to node-pag1/node-pag2; also renamed Get_FilterByResource test method to Get_FilterByNode.
**Files affected:** tests/MetricsHub.Integration.Tests/Events/EventsControllerTests.cs

## [2026-06-07 15:50] Move LogInformation call to after SaveChangesAsync in IngestAsync

**Prompt:** Move the ingestion log entry to after the DB save so it only records events that were successfully persisted.
**Files affected:** src/MetricsHub.Application/Services/Webhooks/WebhookIngestionService.cs

## [2026-06-07 15:45] Suppress CS1591 missing XML doc warning

**Prompt:** Add CS1591 to NoWarn in Directory.Build.props to suppress missing XML documentation comment warnings produced by GenerateDocumentationFile.
**Files affected:** MetricsHub/Directory.Build.props

## [2026-06-07 15:40] Enforce unused usings as build errors via Directory.Build.props

**Prompt:** Configure the solution to treat unused using directives as build errors.
**Files affected:** MetricsHub/Directory.Build.props

## [2026-06-07 15:35] Move MetricsHubException to Application/Exceptions/ folder

**Prompt:** Move MetricsHubException from Normalization/Exceptions/ to a new top-level Exceptions/ folder under Application, updating its namespace and all references.
**Files affected:** src/MetricsHub.Application/Exceptions/MetricsHubException.cs, src/MetricsHub.Application/Normalization/Exceptions/InvalidPayloadException.cs, src/MetricsHub.Application/Normalization/Exceptions/UnknownSourceException.cs, src/MetricsHub.Application/Services/Webhooks/WebhookIngestionService.cs

## [2026-06-07 15:30] Introduce MetricsHubException base class and tighten catch types in IngestAsync

**Prompt:** Create abstract MetricsHubException base class; derive InvalidPayloadException and UnknownSourceException from it; update WebhookIngestionService to catch MetricsHubException in the normalization block and DbUpdateException in the DB block instead of the broad Exception.
**Files affected:** src/MetricsHub.Application/Normalization/Exceptions/MetricsHubException.cs, src/MetricsHub.Application/Normalization/Exceptions/InvalidPayloadException.cs, src/MetricsHub.Application/Normalization/Exceptions/UnknownSourceException.cs, src/MetricsHub.Application/Services/Webhooks/WebhookIngestionService.cs

## [2026-06-07 15:20] Add GlobalExceptionHandler middleware and simplify controller

**Prompt:** Implement IExceptionHandler middleware that maps UnknownSourceException→400, InvalidPayloadException→422, OperationCanceledException→silently handled, everything else→500 with logging; register it in Program.cs; remove the catch blocks from WebhooksController.
**Files affected:** src/MetricsHub.Api/Middleware/GlobalExceptionHandler.cs, src/MetricsHub.Api/Program.cs, src/MetricsHub.Api/Webhooks/WebhooksController.cs

## [2026-06-07 15:10] Add CancellationToken to IngestAsync and update CLAUDE.md

**Prompt:** Add CancellationToken parameter to WebhookIngestionService.IngestAsync and thread it through to SaveChangesAsync and ReadToEndAsync in the controller; add CLAUDE.md rule to always accept and forward CancellationToken in async methods.
**Files affected:** src/MetricsHub.Application/Services/Webhooks/WebhookIngestionService.cs, src/MetricsHub.Api/Webhooks/WebhooksController.cs, CLAUDE.md

## [2026-06-07 15:00] Implement Iteration 5 — polish, logging, and scaffold removal

**Prompt:** Implement Iteration 5: remove WeatherForecast scaffold files; add structured ILogger<T> logging to WebhookIngestionService (info on ingest, warning on normalization failure, error on DB failure) and EventQueryService (info on query parameters); change docker-compose restart policy from on-failure to unless-stopped.
**Files affected:** src/MetricsHub.Api/Controllers/WeatherForecastController.cs (deleted), src/MetricsHub.Api/WeatherForecast.cs (deleted), src/MetricsHub.Application/Services/Webhooks/WebhookIngestionService.cs, src/MetricsHub.Application/Services/Events/EventQueryService.cs, MetricsHub/docker-compose.yml

## [2026-06-07 14:35] Add rule to never commit without explicit user instruction

**Prompt:** Update CLAUDE.md to never create a git commit unless the user explicitly asks for one.
**Files affected:** CLAUDE.md

## [2026-06-07 14:30] Implement Iteration 4 — query endpoint and integration tests

**Prompt:** Implement Iteration 4: EventQuery and PagedResult classes in Application/Services/Events/; EventQueryService with IQueryable filtering chain (from, to, source, resource) and pagination; EventsController (GET /events); register EventQueryService in Program.cs; 6 integration tests covering response structure, source filter, resource filter, time range, and pagination.
**Files affected:** src/MetricsHub.Application/Services/Events/EventQuery.cs, src/MetricsHub.Application/Services/Events/PagedResult.cs, src/MetricsHub.Application/Services/Events/EventQueryService.cs, src/MetricsHub.Api/Events/EventsController.cs, src/MetricsHub.Api/Program.cs, tests/MetricsHub.Integration.Tests/Events/EventsControllerTests.cs

## [2026-06-07 14:10] Commit staged formatting and comment tweaks

**Prompt:** Commit the staged files after minor post-review edits.
**Files affected:** src/MetricsHub.Api/Program.cs, src/MetricsHub.Api/Webhooks/WebhooksController.cs, src/MetricsHub.Application/Services/Webhooks/WebhookIngestionService.cs

## [2026-06-07 14:00] Implement Iteration 3 — webhook endpoint and integration tests

**Prompt:** Implement Iteration 3: WebhookIngestionService in Application/Services/Webhooks/; WebhooksController (POST /webhooks) reading X-Source header; register PulseNormalizer, SentryNormalizer, NormalizerStrategy, and WebhookIngestionService in Program.cs; wrap JsonException in both normalizers so malformed JSON returns 422; add public partial class Program for WebApplicationFactory; write MetricsHubWebApplicationFactory (Testcontainers) and 6 integration tests (happy path Pulse/Sentry, missing header, unknown source, malformed JSON, missing required field).
**Files affected:** src/MetricsHub.Application/Services/Webhooks/WebhookIngestionService.cs, src/MetricsHub.Application/Normalization/Pulse/PulseNormalizer.cs, src/MetricsHub.Application/Normalization/Sentry/SentryNormalizer.cs, src/MetricsHub.Api/Webhooks/WebhooksController.cs, src/MetricsHub.Api/Program.cs, tests/MetricsHub.Integration.Tests/MetricsHubWebApplicationFactory.cs, tests/MetricsHub.Integration.Tests/Webhooks/WebhooksControllerTests.cs

## [2026-06-07 11:25] Update NormalizedEvent model in PLAN.md

**Prompt:** Rename ResourceName to Node, make Region and Environment non-nullable, add RawPayload column, change Source to enum, remove source-specific fields and replace with a JSON Metrics column.
**Files affected:** MetricsHub/PLAN.md

## [2026-06-07 13:35] Add post-change format and test rule to CLAUDE.md

**Prompt:** Update CLAUDE.md so that after every code change, Claude runs dotnet format and dotnet test, fixing issues until all unit tests pass.
**Files affected:** CLAUDE.md

## [2026-06-07 13:30] Mark NormalizedEvent properties as required and set IngestedAt in normalizers

**Prompt:** Mark all non-nullable, non-auto-generated NormalizedEvent properties as required and remove default! initializers; add IngestedAt = DateTimeOffset.UtcNow to both normalizers to satisfy the compiler.
**Files affected:** src/MetricsHub.Domain/NormalizedEvent.cs, src/MetricsHub.Application/Normalization/Pulse/PulseNormalizer.cs, src/MetricsHub.Application/Normalization/Sentry/SentryNormalizer.cs

## [2026-06-07 13:25] Set Pulse CorrelationId to null and update unit test

**Prompt:** Pulse events should always have CorrelationId set to null; update the normalizer and the corresponding unit test.
**Files affected:** src/MetricsHub.Application/Normalization/Pulse/PulseNormalizer.cs, tests/MetricsHub.Unit.Tests/Normalization/PulseNormalizerTests.cs

## [2026-06-07 13:20] Add always-use-braces rule to CLAUDE.md and apply to existing normalizers

**Prompt:** Update CLAUDE.md to always use braces for if/else/for/foreach/using/lock bodies, and fix the braceless if statements in the existing normalizers.
**Files affected:** CLAUDE.md, src/MetricsHub.Application/Normalization/Pulse/PulseNormalizer.cs, src/MetricsHub.Application/Normalization/Sentry/SentryNormalizer.cs

## [2026-06-07 13:15] Add primary constructor rule to CLAUDE.md and apply to existing code

**Prompt:** Update CLAUDE.md to require primary constructors wherever possible, and apply the rule to InvalidPayloadException and UnknownSourceException.
**Files affected:** CLAUDE.md, src/MetricsHub.Application/Normalization/Exceptions/InvalidPayloadException.cs, src/MetricsHub.Application/Normalization/Exceptions/UnknownSourceException.cs

## [2026-06-07 13:10] Move exception classes into Normalization/Exceptions/ folder

**Prompt:** Move InvalidPayloadException and UnknownSourceException into a dedicated Exceptions/ subfolder under Normalization/, updating namespaces and all usings.
**Files affected:** src/MetricsHub.Application/Normalization/Exceptions/InvalidPayloadException.cs, src/MetricsHub.Application/Normalization/Exceptions/UnknownSourceException.cs, src/MetricsHub.Application/Normalization/NormalizerStrategy.cs, src/MetricsHub.Application/Normalization/Pulse/PulseNormalizer.cs, src/MetricsHub.Application/Normalization/Sentry/SentryNormalizer.cs, tests/MetricsHub.Unit.Tests/Webhooks/PulseNormalizerTests.cs, tests/MetricsHub.Unit.Tests/Webhooks/SentryNormalizerTests.cs

## [2026-06-07 13:00] Implement Iteration 2 — normalization strategy pattern and unit tests

**Prompt:** Implement Iteration 2: IEventNormalizer interface, NormalizerStrategy context, InvalidPayloadException, UnknownSourceException, PulsePayload/PulseMetrics/PulseNormalizer, SentryPayload/SentryResource/SentryRule/SentryNormalizer, and unit tests for both normalizers (21 tests, all passing).
**Files affected:** src/MetricsHub.Application/Normalization/IEventNormalizer.cs, src/MetricsHub.Application/Normalization/NormalizerStrategy.cs, src/MetricsHub.Application/Normalization/InvalidPayloadException.cs, src/MetricsHub.Application/Normalization/UnknownSourceException.cs, src/MetricsHub.Application/Normalization/Pulse/PulsePayload.cs, src/MetricsHub.Application/Normalization/Pulse/PulseMetrics.cs, src/MetricsHub.Application/Normalization/Pulse/PulseNormalizer.cs, src/MetricsHub.Application/Normalization/Sentry/SentryPayload.cs, src/MetricsHub.Application/Normalization/Sentry/SentryResource.cs, src/MetricsHub.Application/Normalization/Sentry/SentryRule.cs, src/MetricsHub.Application/Normalization/Sentry/SentryNormalizer.cs, tests/MetricsHub.Unit.Tests/Webhooks/PulseNormalizerTests.cs, tests/MetricsHub.Unit.Tests/Webhooks/SentryNormalizerTests.cs, tests/MetricsHub.Unit.Tests/UnitTest1.cs (deleted), tests/MetricsHub.Integration.Tests/UnitTest1.cs (deleted)

## [2026-06-07 12:20] Add src and tests solution folders to MetricsHub.slnx

**Prompt:** Update the solution file to display projects grouped under src/ and tests/ solution folders.
**Files affected:** MetricsHub/MetricsHub.slnx

## [2026-06-07 12:15] Remove unused Directory.Build.props

**Prompt:** Explained why Directory.Build.props is not needed and deleted it.
**Files affected:** Directory.Build.props (deleted)

## [2026-06-07 12:10] Add one-class-per-file rule to CLAUDE.md and split EventSource into its own file

**Prompt:** Update CLAUDE.md to enforce one class/enum per file, and fix the existing violation in NormalizedEvent.cs.
**Files affected:** CLAUDE.md, src/MetricsHub.Domain/EventSource.cs, src/MetricsHub.Domain/NormalizedEvent.cs

## [2026-06-07 12:00] Implement Iteration 1 — foundation, project structure, EF Core, Docker

**Prompt:** Implement Iteration 1: scaffold src/tests folders, create Domain/Application/Infrastructure/test projects, add NormalizedEvent entity, set up EF Core with Npgsql, write MetricsHubDbContext, update Program.cs with DbContext + health check + migrations, rewrite Dockerfile for multi-project layout, create docker-compose.yml, generate initial EF migration.
**Files affected:** MetricsHub.slnx, Directory.Build.props, docker-compose.yml, src/MetricsHub.Domain/MetricsHub.Domain.csproj, src/MetricsHub.Domain/NormalizedEvent.cs, src/MetricsHub.Application/MetricsHub.Application.csproj, src/MetricsHub.Infrastructure/MetricsHub.Infrastructure.csproj, src/MetricsHub.Infrastructure/MetricsHubDbContext.cs, src/MetricsHub.Infrastructure/Migrations/*, src/MetricsHub.Api/MetricsHub.Api.csproj, src/MetricsHub.Api/Program.cs, src/MetricsHub.Api/appsettings.json, src/MetricsHub.Api/Dockerfile, tests/MetricsHub.Unit.Tests/MetricsHub.Unit.Tests.csproj, tests/MetricsHub.Integration.Tests/MetricsHub.Integration.Tests.csproj

## [2026-06-07 11:20] Reorganize Application project into Services/ and Normalization/ folders in PLAN.md

**Prompt:** In Application, add a Services/ folder (split by feature: Webhooks/, Events/) and a Normalization/ folder (split by feature: Pulse/, Sentry/) for the normalization logic.
**Files affected:** MetricsHub/PLAN.md

## [2026-06-07 11:15] Apply Strategy pattern for normalization and feature-folder layout in PLAN.md

**Prompt:** Use the Strategy pattern for normalization logic and organize code by feature folder (Webhooks, Events) across all projects.
**Files affected:** MetricsHub/PLAN.md

## [2026-06-07 11:10] Split test projects into Unit.Tests and Integration.Tests in PLAN.md

**Prompt:** Split the single test project into MetricsHub.Unit.Tests and MetricsHub.Integration.Tests under tests/.
**Files affected:** MetricsHub/PLAN.md

## [2026-06-07 11:05] Move production projects under src/ and test projects under tests/ in PLAN.md

**Prompt:** Update the solution layout in PLAN.md so production code lives under src/ and test code under tests/.
**Files affected:** MetricsHub/PLAN.md

## [2026-06-07 11:00] Update PLAN.md to use 4-project Clean Architecture with services using DbContext directly

**Prompt:** Revise the architecture to use Domain, Application, Infrastructure, and Api projects; controllers talk to services; services use DbContext directly without a repository layer.
**Files affected:** MetricsHub/PLAN.md

## [2026-06-07 10:30] Write implementation plan for MetricsHub assessment

**Prompt:** Analyze the MetricsHub take-home assessment requirements, ask clarifying questions, propose an architecture, and write a PLAN.md with the implementation split into small iterations.
**Files affected:** MetricsHub/PLAN.md

## [2026-06-07 00:00] Add rule to never include AI signature in commit messages
**Prompt:** Update CLAUDE.md to never add a Co-Authored-By or AI signature line in commit messages.
**Files affected:** CLAUDE.md
