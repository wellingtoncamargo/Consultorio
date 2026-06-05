Quality Workflow

1. Input: feature spec or target patterns (glob)
2. Agent validates spec and scans target files
3. Agent generates unit test skeletons in src\Consultorio.Tests\Consultorio.Tests.Unit
4. Run local build and test
5. Run mutation testing (Stryker.NET) per configured thresholds
6. Produce report and create PR with tests + report
7. Human reviews PR; on request agent iterates

Checkpoints:
- After test generation (human review)
- After Stryker run (review mutants)
