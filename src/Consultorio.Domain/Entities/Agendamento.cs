using Consultorio.Domain.Enums;

namespace Consultorio.Domain.Entities
{
    public class Agendamento
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PacienteId { get; set; }
        public Guid MedicoId { get; set; }
        public Guid LocalTrabalhoId { get; set; }
        public DateTime DataAgendamento { get; set; }
        public TimeSpan HoraAgendamento { get; set; }
        public int DuracaoMinutos { get; set; } = 30;
        public StatusAgendamento Status { get; set; } = StatusAgendamento.Agendado;
        public string Motivo { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public bool NotificadoPorEmail { get; set; }
        public bool NotificadoPorWhatsApp { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }

        public Paciente? Paciente { get; set; }
        public Medico? Medico { get; set; }
        public LocalTrabalho? LocalTrabalho { get; set; }

        public DateTime DataHoraInicio => DataAgendamento.Date + HoraAgendamento;
        public DateTime DataHoraFim => DataHoraInicio.AddMinutes(DuracaoMinutos);

        public string HorarioFormatado =>
            $"{HoraAgendamento:hh\\:mm} - {DataHoraFim:HH:mm}";

        public string StatusDescricao => Status switch
        {
            StatusAgendamento.Agendado => "Agendado",
            StatusAgendamento.Confirmado => "Confirmado",
            StatusAgendamento.Realizado => "Realizado",
            StatusAgendamento.Cancelado => "Cancelado",
            StatusAgendamento.NaoCompareceu => "Não Compareceu",
            _ => "Desconhecido"
        };

        public bool PodeEditar => Status == StatusAgendamento.Agendado || Status == StatusAgendamento.Confirmado;
        public bool PodeCancelar => Status != StatusAgendamento.Cancelado && Status != StatusAgendamento.Realizado;
    }
}

