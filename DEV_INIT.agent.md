---
name: dev-init
description: "Agente 'dev-init' para inicializar tarefas de desenvolvimento: CI, testes, migrations, Docker, OpenAPI, instrumentação e documentação."
applyTo: "*"
author: "AI assistant using Copilot CLI runtime in VS Code"
---

Objetivo
- Automatizar as tarefas iniciais para preparar o projeto "Consultorio" para desenvolvimento colaborativo e CI.

Escopo inicial
1. Criar pipeline CI (build + tests) e adicionar badge no README.
2. Padronizar e ampliar testes (unit/integration) e adicionar cobertura mínima.
3. Configurar EF Core migrations e scripts de seed (apenas dev, sem PII reais).
4. Adicionar Dockerfile(s) e docker-compose para dev local.
5. Melhorar OpenAPI (exemplos, schemas) e adicionar endpoints de health/auth simulada.
6. Instrumentação básica: logs estruturados, health checks e métricas.
7. Criar CONTRIBUTING.md, ISSUE_TEMPLATE e backlog inicial de tarefas pequenas.

Regras de operação
- Executar cada item como commit independente com mensagem clara.
- Após cada mudança: executar `dotnet build` e `dotnet test` e reportar saída.
- Não commitar segredos: usar variáveis de ambiente; registrar onde colocá-las.
- Não incluir dados pessoais reais nos seeds; usar dados sintéticos.

Expectativa de entregáveis
- Workflows em `.github/workflows/` (YAML) para CI.
- Dockerfile(s) em `src/Consultorio.API/` e `docker-compose.yml` na raiz.
- Scripts de migração em `src/Consultorio.Data/Migrations`.
- Arquivos de documentação: `CONTRIBUTING.md`, `docs/SETUP.md` atualizado.

Referências
- AGENT_PLAN.md (root)

Quando executar
- Agente pode rodar em modo interativo com aprovação do mantenedor para cada PR criado.