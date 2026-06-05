Service Test Scenarios — Detailed (priority + mutation targets)

Overview

This document lists exhaustive unit-test scenarios for the Services layer, prioritized and annotated with suggested mutation targets (for Stryker.NET). Use these to generate unit tests in src\Consultorio.Tests\Consultorio.Tests.Unit and to guide mutation test selection.

Priority legend
- High: must-have unit tests (business rules, data integrity, security)
- Medium: important branches and error paths
- Low: optional edge cases, formatting, presentation logic

Common mutation targets
- Conditional/Relational (ROR, COR): if/return conditions, boundary checks
- Logic/Boolean (LCR): invert boolean branches
- Return values (RV): change return constants
- Method calls (MCR): remove or replace calls (e.g., repo.Save)
- Null checks (NUL): null-handling

1) PacienteService
- High
  - SalvarAsync: Nome vazio -> returns (false, "Nome")
    - Test: input Nome null/empty
    - Mutation focus: change "string.IsNullOrWhiteSpace" to always false (NUL/COR)
  - SalvarAsync: CPF duplicate -> returns error
    - Test: mock repo GetByCPFAsync returns existing with different Id
    - Mutation focus: equality checks (ROR)
  - SalvarAsync: create new (Id empty) -> calls AddAsync, sets Id/DataCriacao/DataAtualizacao
    - Test: verify AddAsync invoked and Id != Guid.Empty
    - Mutation focus: method call removal (MCR), SaveChanges invocation
  - ExcluirAsync: nonexistent -> return false
    - Test: repo GetByIdAsync returns null
    - Mutation focus: branch invert (LCR)
  - ExcluirAsync: existing -> sets Ativo false and updates
    - Test: verify UpdateAsync called and Ativo false
- Medium
  - BuscarAsync: null/empty termo -> expected behavior (pass-through)
  - GetAllAsync ordering (ensure ordering by Nome)
- Low
  - Date updates correctness (DataAtualizacao set to near-now)

2) MedicoService
- High
  - SalvarAsync: Nome/CRM required -> returns false when missing
  - SalvarAsync: CRM duplicate -> error
  - SalvarAsync: create vs update behavior (AddAsync vs UpdateAsync)
  - ExcluirAsync: same patterns as PacienteService
- Medium
  - GetComLocaisAsync: calls underlying repo include behavior (mock return null/with children)
- Mutation targets: CRM equality, required-field checks, method calls (MCR)

3) LocalTrabalhoService
- High
  - SalvarAsync: Nome required
  - SalvarAsync: MedicoId required (Guid.Empty detection)
  - ExcluirAsync: sets Ativo false
- Medium
  - GetPorMedicoAsync: filters by MedicoId
- Mutation: Guid.Empty checks (NUL/COR), inclusion of medico (MCR)

4) AgendamentoService (High-risk business logic)
- High
  - SalvarAsync: Paciente/Médico/Local required -> returns false when missing
  - SalvarAsync: DataAgendamento default -> returns false
  - SalvarAsync: VerificarConflitoAsync true -> prevent creation
    - Test: mock repo.VerificarConflitoAsync true for same time
    - Mutation focus: time overlap logic (ROR) and inequality (>= vs >)
  - SalvarAsync: valid creation -> AddAsync called, Id set
  - CancelarAsync: returns false when not found or !PodeCancelar
  - ConfirmarAsync/RealizarAsync: set Status accordingly and UpdateAsync invoked
- Medium
  - GetPorPeriodo/GetPorMedico: verify repository call mapping
- Low
  - Notification flows commented — plan tests if enabled
- Mutation targets: time calculations (add/sub minutes), condition inversion, excludedId checks

5) HistoricoService
- High
  - SalvarAsync: PacienteId/MedicoId/DataConsulta required
  - ExcluirAsync: not-found vs delete success
- Medium
  - GetPorPeriodo logic
- Mutation targets: required field checks, method call removals

6) NotificacaoService (external IO; test behavior, not SMTP)
- High
  - EnviarEmailAgendamentoAsync: return false when Paciente.Email null/empty
  - EnviarEmailAgendamentoAsync: return false when SMTP.Configurado false
- Medium
  - EnviarEmailAgendamentoAsync: when SMTP configured and email present, ensure method tries to send and returns true or false upon exception
    - Use SmtpClient wrapper abstraction or mock via dependency injection — if no wrapper exists, verify that method catches exceptions and returns false
- Low
  - GerarCorpoEmail/WhatsApp message formatting (string contains expected tokens)
- Mutation targets: conditions testing (IsNullOrWhiteSpace), exception swallowing (ensure catch block behavior), templated string content

7) TokenService
- High
  - GenerateAccessToken: returns non-empty token and contains expected claims (sub/email/role)
  - ValidateToken: valid token returns principal; invalid token returns null
- Medium
  - Token expiration: ValidateToken returns null for expired token (simulate via small expiration time)
- Mutation targets: claims creation, issuer/audience validation (change constants), ClockSkew handling

8) PasswordHasherService
- High
  - HashPassword then Verify returns true for correct password, false otherwise
  - Verify handles malformed hashed string gracefully (returns false)
- Mutation targets: salt/key split (split limit), FixedTimeEquals equality (force false/true)

9) Repositories (BaseRepository and derived)
- High
  - BaseRepository AddAsync: calls DbSet.AddAsync and SaveChangesAsync
  - UpdateAsync/DeleteAsync behavior handling nulls
  - Custom query methods: BuscarPorNomeAsync uses ToLower/Contains -> test case-insensitivity
- Medium
  - VerificarConflitoAsync logic in AgendamentoRepository: time overlap calculations
- Mutation: Where predicate boundaries, inclusive/exclusive date comparisons, LINQ expressions

Cross-cutting scenarios
- Exception handling: repositories or DbContext throw — services should propagate or convert to controlled failure responses
- Concurrency: SaveChanges exceptions during Add/Update — ensure tests simulate exceptions and assert behavior
- Date/Time: tests should use fixed DateTimes (injectable now provider if available) or assert relative behaviors with tolerances

Suggested prioritization for test generation (initial run)
1. AgendamentoService (highest business-risk)
2. PacienteService, MedicoService
3. Repositories: AgendamentoRepository time conflict, PacienteRepository CPF unique
4. TokenService, PasswordHasherService
5. NotificacaoService (behavioral tests without real SMTP)
6. HistoricoService, LocalTrabalhoService

Mutation testing guidance
- For each High-priority test class, include Stryker target list limited to the service source file to reduce runtime.
- Start with small subsets (e.g., AgendamentoService only) to iterate on surviving mutants and add tests that replicate scenarios that kill them.
- Use Stryker config in agent/stryker/stryker-config.json; run with timeout and per-file mutate patterns.

Deliverables
- services-test-scenarios.md (this file) in agent/specs
- Mapping table for tests -> Stryker mutate patterns (generated on demand)

Next step
- Generate the actual test files for the top 3 priority targets (AgendamentoService, PacienteService, MedicoService) and run tests locally.
