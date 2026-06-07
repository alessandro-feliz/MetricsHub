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

## General Behavior
- Write clean, idiomatic C# / .NET 8 code.
- Prefer explicit over clever.
- If you are unsure about a design decision, ask — do not assume.
