# Assessment Governance
You are helping a candidate complete a .NET take-home assessment. Follow these rules for every interaction.

## Auto-Logging — Required
After EVERY interaction where you create or modify files, append an entry to AI-PROMPT-LOG.md in the project root. Use this exact format:

## [YYYY-MM-DD HH:MM] <one-line summary of what was requested>
**Prompt:** <paraphrase of what the user asked you to do>
**Files affected:** <comma-separated list of files created or modified>

Rules:
- Log EVERY interaction that touches files — no exceptions.
- Use the current timestamp.
- The prompt field should be a concise paraphrase, not a verbatim copy.
- List only files you actually created or modified, not files you merely read.
- Do NOT log read-only interactions (questions, explanations with no file changes).
- Never edit or delete previous log entries.
- If the log file does not exist, create it with a # AI Prompt Log header before the first entry.

## Git Commits
- Never add a `Co-Authored-By` or any Claude/AI signature line to commit messages.
- Never create a git commit unless the user explicitly asks for one.

## General Behavior
- Write clean, idiomatic C# / .NET 8 code.
- Prefer explicit over clever.
- One class or enum per file — no exceptions.
- Use primary constructors whenever possible (C# 12).
- Always use braces for the body of if/else, for, foreach, using, and lock statements — no single-line omissions.
- If you are unsure about a design decision, ask — do not assume.

## After Every Code Change
1. Run `dotnet format` on the solution to format all changed files.
2. Run `dotnet test tests/MetricsHub.Unit.Tests/MetricsHub.Unit.Tests.csproj` to execute unit tests.
3. If any tests fail, fix the root cause and repeat from step 1 until all tests pass.
