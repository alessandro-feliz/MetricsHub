# AI Prompt Log

## [2026-06-07 11:25] Update NormalizedEvent model in PLAN.md

**Prompt:** Rename ResourceName to Node, make Region and Environment non-nullable, add RawPayload column, change Source to enum, remove source-specific fields and replace with a JSON Metrics column.
**Files affected:** MetricsHub/PLAN.md

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
