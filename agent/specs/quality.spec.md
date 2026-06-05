Quality Agent Spec

Objetivo:
Fornecer regras e contratos para geração de testes unitários e execução de testes de mutação.

Entradas:
- Spec da feature/entidade (arquivo MD ou JSON Schema)
- Path/pattern para selecionar classes (glob) ou 'todos'
- Nível de profundidade (unitário, integração)
- Política de cobertura e thresholds de mutação (ex: >= 80% coverage, 70% mutation score)

Saídas:
- Test files criados em: src\Consultorio.Tests\Consultorio.Tests.Unit
- Relatório de execução do Stryker.NET (mutations, score)
- PR com mudanças e relatório resumido

Regras de geração de testes:
- Seguir arrange-act-assert
- Usar mocks para dependências externas (Moq)
- Usar fixtures e TestServer para integração
- Nomear testes: {ClassName}_{MethodName}_Should_{ExpectedBehavior}

Mutation testing:
- Integrar com Stryker.NET usando agent/stryker/stryker-config.json
- Permitir seleção interativa de classes/patterns
- Falhar workflow se mutation score abaixo do threshold
