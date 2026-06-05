Consultorio Quality Agent

Propósito:
Agente especializado em qualidade que gera testes unitários seguindo a estrutura do projeto e executa testes de mutação.

Organização:
- agent/specs: especificações e contratos para geração de testes
- agent/skills: skills reutilizáveis (gerar testes, configurar mutação)
- agent/prompts: templates de prompts para chamadas confiáveis
- agent/workflows: workflows orquestrados com checkpoints e revisão humana
- agent/stryker: configuração para Stryker.NET (mutation testing)
- agent/templates: templates de teste e fixtures

Comportamento principal:
- Gerar testes unitários na pasta src\Consultorio.Tests\Consultorio.Tests.Unit
- Ser agnóstico a pastas/classes; permitir seleção via pattern/entrada interativa
- Criar PRs com testes gerados e executar Stryker.NET conforme policy

Revisão humana:
- Todo PR requer revisão e aprovação antes de merge
