Medicos Spec (merged)

Resumo:
Contratos para CRUD de médicos e relacionamento com agendamentos.

Model:
- Medico: { Id: GUID, Nome: string, CRM: string, Especialidade: string, Telefone?: string }

Regras de negócio:
- CRM único
- Especialidade obrigatória
- Referência FK para Agendamento

Aceitação:
- Swagger com exemplos
- Testes unitários para validação de CRM e criação
- Integração: listagem, paginação opcional
