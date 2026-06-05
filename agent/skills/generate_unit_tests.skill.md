Skill: generate_unit_tests

Propósito:
Gerar testes unitários para classes/entidades seguindo a spec do projeto.

Inputs:
- target_patterns: glob ou lista de arquivos (ex: src/**/Services/*.cs)
- spec_path: caminho para spec de feature (opcional)
- test_project: default src\Consultorio.Tests\Consultorio.Tests.Unit
- style: unit/integration
- author: nome para PR

Outputs:
- Arquivos de teste gerados no projeto de testes
- Fixtures, TestDataBuilders e AutoMapper config se necessário
- Lista de arquivos criados + diff para PR

Passos:
1. Ler spec e inferir contratos (nomes, parâmetros, comportamentos)
2. Mapear métodos públicos e gerar cenários de sucesso/erro
3. Gerar testes xUnit/NUnit conforme convenção do repo (detectar automaticamente)
4. Registrar arquivos gerados em staging e criar PR (requer aprovação)

Práticas:
- Incluir arranjos mínimos e asserts claros
- Usar Moq para dependências
- Não alterar código de produção sem aprovação humana
