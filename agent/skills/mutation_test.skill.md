Skill: mutation_testing

Propósito:
Configurar e executar testes de mutação usando Stryker.NET e gerar relatórios para revisão.

Inputs:
- target_patterns: glob ou lista de arquivos a serem mutados
- test_project: src\Consultorio.Tests\Consultorio.Tests.Unit
- thresholds: { mutation: 70, high: 80, break: 60 }
- run_timeout: segundos

Outputs:
- Relatório Stryker (HTML/JSON)
- Mutation score summary
- Listagem de mutants sobreviventes para priorização

Passos:
1. Preparar stryker config com targets
2. Executar dotnet stryker --config-file path
3. Coletar relatório e interpretar scores
4. Se score < break threshold, marcar PR como requiring-fixes

Observações:
- Agent não modifica o código fonte original; sugere testes para matar mutants
- Permitir reexecução incremental de Stryker para subsets
