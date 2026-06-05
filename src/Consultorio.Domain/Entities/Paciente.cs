using System.Collections.Generic;

namespace Consultorio.Domain.Entities
{
    public class Paciente
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; } = string.Empty;
        public string RG { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Complemento { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string CEP { get; set; } = string.Empty;
        public string Observacoes { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataAtualizacao { get; set; }

        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public ICollection<Historico> Historicos { get; set; } = new List<Historico>();

        public int Idade => DateTime.Today.Year - DataNascimento.Year -
            (DateTime.Today < DataNascimento.AddYears(DateTime.Today.Year - DataNascimento.Year) ? 1 : 0);

        public string NomeCompleto => Nome;
        public string EnderecoCompleto =>
            string.Join(", ", new[] { Endereco, Numero, Complemento, Bairro, Cidade, Estado }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}

