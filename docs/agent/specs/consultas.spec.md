Consultas/Agendamentos Spec (merged)

Resumo:
Definição de contrato para agendamentos.

Model:
- Agendamento: { Id: GUID, PacienteId: GUID, MedicoId: GUID, DataHora: datetime, DuracaoMinutos: int, LocalId?: GUID, Status: string }

Regras:
- Evitar conflito de horário para o mesmo médico
- Validar existência de Paciente e Medico
- Validar horário do local

Aceitação:
- Testes de conflito de horário
- Endpoints com exemplos de payload e respostas de erro
