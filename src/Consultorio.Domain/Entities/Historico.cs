
namespace Consultorio.Domain.Entities
{
    public class Historico
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PacienteId { get; set; }
        public Guid MedicoId { get; set; }
        public Guid? AgendamentoId { get; set; }
        public DateTime DataConsulta { get; set; }
        public string Diagnostico { get; set; } = string.Empty;
        public string Tratamento { get; set; } = string.Empty;
        public string Medicacoes { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public string Receita { get; set; } = string.Empty;
        public string Anamnese { get; set; } = string.Empty;
        public string ExamesRealizados { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }

        public Paciente? Paciente { get; set; }
        public Medico? Medico { get; set; }
    }
}

