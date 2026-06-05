Agent Workflows (merged)

Workflows:
- Implement feature from spec -> generate code -> run tests -> create PR
- Create tests from spec -> run CI locally -> report
- Generate MAUI page -> integrate -> run UI smoke tests

Each workflow:
- Has checkpoints and requires human approval before merge
- Produces plan.md and a changelog entry
