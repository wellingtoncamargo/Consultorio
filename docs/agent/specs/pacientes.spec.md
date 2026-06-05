Pacientes Spec (merged)

Resumo:
Contrato de API e critérios de aceitação para paciente.

Model:
- Paciente: { Id: GUID, Nome: string, CPF: string, DataNascimento: date, Telefone?: string, Email?: string }

Endpoints:
- GET /api/pacientes
- GET /api/pacientes/{id}
- POST /api/pacientes
- PUT /api/pacientes/{id}
- DELETE /api/pacientes/{id}

Regras:
- CPF único e validado
- Datas válidas (nascimento < hoje)
- Campos obrigatórios: Nome, CPF

Testes de aceitação:
- Contrato JSON com exemplos
- Integração CRUD completa
- Casos de borda: duplicata de CPF, payload inválido
