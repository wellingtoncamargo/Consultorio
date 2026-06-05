using System.Collections.Generic;

namespace Consultorio.Domain.Entities
{
    public class LocalTrabalho
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MedicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public TimeSpan HorarioAbertura { get; set; } = new TimeSpan(8, 0, 0);
        public TimeSpan HorarioFechamento { get; set; } = new TimeSpan(18, 0, 0);
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }

        public Medico? Medico { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();

        public string EnderecoCompleto =>
            string.Join(", ", new[] { Endereco, Numero, Complemento, Bairro, Cidade, Estado }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

        public string HorarioFormatado =>
            $"{HorarioAbertura:hh\\:mm} - {HorarioFechamento:hh\\:mm}";
    }
}

