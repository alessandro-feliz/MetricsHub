\# AI Reflection



\## What did AI handle well?

The AI was effective at producing an initial implementation plan and implementing it once the requirements were clarified and I provided additional guidance on the expected code structure. It helped accelerate early development and provided a useful starting point for structuring the solution.



\## Where did you override AI?

The initial implementation was too simplistic, so I introduced a more complete architectural structure, including project organization, strategy patterns, middleware for exception handling, and DTO definitions.

Some generated code was difficult to read, so I added rules in CLAUDE.md to enforce clearer and more consistent syntax.

At one point, the AI also began committing code automatically, which I restricted by updating project rules to ensure explicit human control over commits.

Additionally, the AI often assumed request inputs were valid, which led to missing validations and insufficient test coverage. I corrected this by enforcing explicit validation logic and adding tests for common and edge-case scenarios.



\## What did you write without AI?

My contribution was primarily supervisory rather than manual coding. I reviewed AI-generated implementations, validated their correctness against requirements, and intervened when needed to correct assumptions, improve design decisions, or guide alternative implementations.



