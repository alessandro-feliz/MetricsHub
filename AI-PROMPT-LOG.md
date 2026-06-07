# AI Prompt Log

## [2026-06-07 11:25] Update NormalizedEvent model in PLAN.md

**Prompt:** Rename ResourceName to Node, make Region and Environment non-nullable, add RawPayload column, change Source to enum, remove source-specific fields and replace with a JSON Metrics column.
**Files affected:** MetricsHub/PLAN.md

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
