# AI Prompt Log

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
