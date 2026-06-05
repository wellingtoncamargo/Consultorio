# DEV_INIT Instructions

Orientação atualizada para o agente `dev-init` com base na inspeção e execução do projeto.

Resumo das mudanças recentes
- CI workflow criado em `.github/workflows/ci.yml` (build + tests). Nota: workflow usa `dotnet-version: '9.0.x'`.
- Testes unitários ajustados para `net8.0` para compatibilidade local; integração: projeto criado em `tests/Consultorio.Tests.Integration` (ainda sem testes).
- Registro DI: `Consultorio.Services.ConfiguracaoSmtp` e `NotificacaoService` foram registrados em `src/Consultorio.API/Program.cs`.
- API executável localmente: `dotnet run --project src/Consultorio.API` inicia e cria o banco SQLite automaticamente (ouvido em https://localhost:37311).

Regras operacionais (mantidas)
- Commits pequenos, um propósito por commit. Mensagens claras: `feat(ci): ...`, `fix(di): register notificacao`.
- Rodar build e testes após cada mudança e anexar logs de falha ao PR.
- Seeds: apenas dados sintéticos; documentar como resetar DB.
- Segredos: variáveis de ambiente; documentar variáveis necessárias (JWT, SMTP, DB).

Comandos recomendados atualizados
- Build: `dotnet build ./src/Consultorio.API/Consultorio.API.csproj`
- Run API (dev): `dotnet run --project src/Consultorio.API/Consultorio.API.csproj`
- Test unitários: `dotnet test tests\Consultorio.Tests.Unit\Consultorio.Tests.Unit.csproj`
- Test integração: `dotnet test tests\Consultorio.Tests.Integration\Consultorio.Tests.Integration.csproj` (adicionar testes neste projeto)
- Migrations: `dotnet ef migrations add InitialCreate -p src/Consultorio.Data -s src/Consultorio.API`
- Apply migrations (dev): `dotnet ef database update -p src/Consultorio.Data -s src/Consultorio.API`

Notas de compatibilidade
- Projeto global visa `net9.0` (Consultorio.API). Se ambiente local não tiver .NET 9 SDK, ajustar projetos para `net8.0` temporariamente ou instalar .NET 9.
- CI está configurada para usar .NET 9; alinhar runner se mudar targets.

Próximos passos recomendados
1. Adicionar health-checks e endpoint `/health`.
2. Inserir configuração SMTP segura em `appsettings.Development.json` (ou variáveis de ambiente) se desejado.
3. Criar testes de integração no projeto criado.
4. Commitar e abrir PR com checklist (build, tests, migration, docs).

Referências
- AGENT_PLAN.md
- DEV_INIT.agent.md

--
Atualizado por AI assistant using Copilot CLI runtime in VS Code.