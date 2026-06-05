Skill: generate_unit_tests (v2)

Propósito:
Gerar testes unitários para classes/entidades seguindo a spec do projeto e organizá-los por tipo (Services, Repositories, Controllers, Application, Domain) dentro do projeto de testes.

Inputs:
- target_patterns: glob ou lista de arquivos (ex: src/**/Services/*.cs)
- spec_path: caminho para spec de feature (opcional)
- test_project: default src\Consultorio.Tests\Consultorio.Tests.Unit
- style: unit/integration
- author: nome para PR
- place_by_type: boolean (default true) — quando true, organiza arquivos em subpastas por tipo
- template: opcional nome de template em agent/templates (default: test-file-v2.template.md)

Outputs:
- Arquivos de teste gerados no projeto de testes, colocados em {test_project}\{TypeFolder}\
- Fixtures, TestDataBuilders e AutoMapper config se necessário
- Lista de arquivos gerados + diff para PR

Passos:
1. Ler spec e inferir contratos (nomes, parâmetros, comportamentos).
2. Mapear métodos públicos e gerar cenários de sucesso/erro (success, validation, exceptions, edge cases).
3. Determinar TestFolder (TypeFolder) a partir do caminho do arquivo alvo:
   - se o caminho contiver "\\Services\\" → TypeFolder = Services
   - se o caminho contiver "\\Repositories\\" → TypeFolder = Repositories
   - se o caminho contiver "\\Controllers\\" → TypeFolder = Controllers
   - se o caminho contiver "\\Application\\" → TypeFolder = Application
   - se o caminho contiver "\\Domain\\" → TypeFolder = Domain
   - caso contrário, TypeFolder = Misc
4. Gerar arquivo de teste usando o template (agent/templates/{template}) e substituir placeholders:
   - {ClassName}, {Mocks}, {SystemUnderTestDeclaration}, {SetupMocks}, {SutInitialization}, {ScenarioName}, {Arrange}, {SutInstance}, {MethodUnderTest}, {MethodArgs}, {Asserts}
5. Escrever arquivo em: {test_project}\{TypeFolder}\{ClassName}Tests.cs e garantir namespace: Consultorio.Tests.Unit.{TypeFolder}
6. Atualizar usings (global usings ou específicos) conforme o código testado.
7. Executar dotnet build e dotnet test no test_project para validar compilação e testes básicos.
8. Registrar arquivos gerados e preparar diff/PR para revisão.

Práticas:
- Incluir arranjos mínimos e asserts claros
- Usar Moq para dependências
- Não alterar código de produção sem aprovação humana
- Evitar duplicação: se arquivo de teste existir, acrescentar novos cenários em vez de sobrescrever (salvo quando for regeneração explícita)

Notas de implementação do agente:
- O gerador deve mapear dependências do construtor para criar mocks automaticamente.
- Quando place_by_type=true, criar a subpasta se não existir.
- Detectar framework de teste (NUnit/xUnit) pelo projeto de testes e adaptar assert/usings.
- Preferir o template test-file-v2.template.md que suporta placeholders de pasta/namespace.
