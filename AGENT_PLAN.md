# Plano de Melhoria — Agente de Desenvolvimento Inicial

Objetivo: fornecer contexto e um plano inicial para um agente de IA que coordene as tarefas de preparação do projeto "Consultorio".

Visão geral:
- Repositório .NET 9 multi-projeto (API, Domain, Data, Services, MAUI, Tests).
- Objetivos imediatos: garantir build CI, testes, banco de dados com migrations, containerização, OpenAPI melhorada, e documentação.

Tarefas propostas (prioridade):
1. Criar CI (build + run tests) e badges.
2. Padronizar e aumentar testes unitários e de integração; adicionar cobertura de testes.
3. Configurar EF Core migrations, seed de dados de teste (no ambiente de desenvolvimento apenas).
4. Adicionar Dockerfile(s) para API e compor docker-compose para desenvolvimento local.
5. Expandir OpenAPI: exemplos, esquemas e endpoints de autenticação simulada para testes.
6. Instrumentação: logs estruturados, health checks, e métricas básicas (Prometheus/otel).
7. Criar CONTRIBUTING.md, ISSUE_TEMPLATE e tarefas (backlog) para automação pelo agente.

Regras de segurança:
- Nunca incluir dados pessoais reais nos seeds.
- Armazenar segredos em variáveis de ambiente; documentar onde colocar.

Instruções para o agente (dev-init):
- Executar as tarefas na ordem de prioridade, criando commits separados para cada item.
- Executar build e testes após cada mudança; reportar falhas com saída de logs.
- Abrir PRs com descrições curtas e checklist de verificação.

Anexos e recursos:
- Referenciar arquivos docs/API.md, ARCHITECTURE.md e DATABASE.md antes de mudanças estruturais.

Próximo passo: criar arquivos de configuração do agente (.agent.md / .instructions.md) contendo estes objetivos e autorizações mínimas para atuar.

--
Gerado por AI assistant using Copilot CLI runtime in VS Code.