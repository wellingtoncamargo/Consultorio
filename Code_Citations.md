# Code Citations - Consultorio

## Entities - Domain Models

### Paciente.cs

```csharp
// filepath: src\Consultorio.Domain\Entities\Paciente.cs
using System;
using System.Collections.Generic;

namespace Consultorio.Domain.Entities
{
    public class Paciente
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public DateTime DataNascimento { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string Observacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public ICollection<Historico> Historicos { get; set; } = new List<Historico>();
    }
}
```

### Medico.cs

```csharp
// filepath: src\Consultorio.Domain\Entities\Medico.cs
using System;
using System.Collections.Generic;

namespace Consultorio.Domain.Entities
{
    public class Medico
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string CRM { get; set; }
        public string Especialidade { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string Observacoes { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public ICollection<LocalTrabalho> LocaisTrabalho { get; set; } = new List<LocalTrabalho>();
        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
        public ICollection<Historico> Historicos { get; set; } = new List<Historico>();
    }
}
```

### LocalTrabalho.cs

```csharp
// filepath: src\Consultorio.Domain\Entities\LocalTrabalho.cs
using System;
using System.Collections.Generic;

namespace Consultorio.Domain.Entities
{
    public class LocalTrabalho
    {
        public Guid Id { get; set; }
        public Guid MedicoId { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string Telefone { get; set; }
        public TimeSpan HorarioAbertura { get; set; }
        public TimeSpan HorarioFechamento { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public Medico Medico { get; set; }
        public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
    }
}
```

### Agendamento.cs

```csharp
// filepath: src\Consultorio.Domain\Entities\Agendamento.cs
using System;
using Consultorio.Domain.Enums;

namespace Consultorio.Domain.Entities
{
    public class Agendamento
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public Guid MedicoId { get; set; }
        public Guid LocalTrabalhoId { get; set; }
        public DateTime DataAgendamento { get; set; }
        public TimeSpan HoraAgendamento { get; set; }
        public int DuracaoMinutos { get; set; }
        public StatusAgendamento Status { get; set; }
        public string Motivo { get; set; }
        public string Observacoes { get; set; }
        public bool NotificadoPorEmail { get; set; }
        public bool NotificadoPorWhatsApp { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public Paciente Paciente { get; set; }
        public Medico Medico { get; set; }
        public LocalTrabalho LocalTrabalho { get; set; }
    }
}
```

### Historico.cs

```csharp
// filepath: src\Consultorio.Domain\Entities\Historico.cs
using System;

namespace Consultorio.Domain.Entities
{
    public class Historico
    {
        public Guid Id { get; set; }
        public Guid PacienteId { get; set; }
        public Guid MedicoId { get; set; }
        public DateTime DataConsulta { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamento { get; set; }
        public string Medicacoes { get; set; }
        public string Observacoes { get; set; }
        public string Receita { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }

        public Paciente Paciente { get; set; }
        public Medico Medico { get; set; }
    }
}
```

## Enums

### StatusAgendamento.cs

```csharp
// filepath: src\Consultorio.Domain\Enums\StatusAgendamento.cs
namespace Consultorio.Domain.Enums
{
    public enum StatusAgendamento
    {
        Pendente = 1,
        Confirmado = 2,
        Realizado = 3,
        Cancelado = 4,
        Remarcado = 5,
        NaoCompareceu = 6
    }
}
```

## Data Context

### ConsultorioDbContext.cs

```csharp
// filepath: src\Consultorio.Data\Context\ConsultorioDbContext.cs
using Microsoft.EntityFrameworkCore;
using Consultorio.Domain.Entities;

namespace Consultorio.Data.Context
{
    public class ConsultorioDbContext : DbContext
    {
        public ConsultorioDbContext(DbContextOptions<ConsultorioDbContext> options) : base(options)
        {
        }

        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<LocalTrabalho> LocaisTrabalho { get; set; }
        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Historico> Historicos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Paciente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.CPF).HasMaxLength(11);
                entity.Property(e => e.Telefone).HasMaxLength(20);
                entity.Property(e => e.Celular).HasMaxLength(20);
                entity.HasMany(e => e.Agendamentos).WithOne(a => a.Paciente).HasForeignKey(a => a.PacienteId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Historicos).WithOne(h => h.Paciente).HasForeignKey(h => h.PacienteId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Medico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.CPF).HasMaxLength(11);
                entity.Property(e => e.CRM).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Especialidade).HasMaxLength(255);
                entity.HasMany(e => e.LocaisTrabalho).WithOne(l => l.Medico).HasForeignKey(l => l.MedicoId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Agendamentos).WithOne(a => a.Medico).HasForeignKey(a => a.MedicoId).OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(e => e.Historicos).WithOne(h => h.Medico).HasForeignKey(h => h.MedicoId).OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<LocalTrabalho>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Endereco).HasMaxLength(500);
                entity.HasMany(e => e.Agendamentos).WithOne(a => a.LocalTrabalho).HasForeignKey(a => a.LocalTrabalhoId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Agendamento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Motivo).HasMaxLength(500);
                entity.Property(e => e.Status).HasConversion<int>();
                entity.HasIndex(e => new { e.PacienteId, e.DataAgendamento });
                entity.HasIndex(e => new { e.MedicoId, e.DataAgendamento });
            });

            modelBuilder.Entity<Historico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Diagnostico).HasMaxLength(1000);
                entity.Property(e => e.Tratamento).HasMaxLength(1000);
                entity.Property(e => e.Medicacoes).HasMaxLength(1000);
                entity.HasIndex(e => new { e.PacienteId, e.DataConsulta });
            });
        }
    }
}
```

## Tests

### PacienteServiceTests.cs

```csharp
// filepath: tests\Consultorio.Tests.Unit\Services\PacienteServiceTests.cs
using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Consultorio.Domain.Entities;
using Consultorio.Services;

namespace Consultorio.Tests.Unit.Services
{
    [TestFixture]
    public class PacienteServiceTests
    {
        private Mock<IPacienteRepository> _pacienteRepositoryMock;
        private PacienteService _pacienteService;

        [SetUp]
        public void Setup()
        {
            _pacienteRepositoryMock = new Mock<IPacienteRepository>();
            _pacienteService = new PacienteService(_pacienteRepositoryMock.Object);
        }

        [Test]
        public async Task CriarPaciente_ComDadosValidos_DeveRetornarPacienteComId()
        {
            var paciente = new Paciente
            {
                Nome = "João Silva",
                Email = "joao@example.com",
                CPF = "12345678901"
            };

            _pacienteRepositoryMock.Setup(r => r.AdicionarAsync(It.IsAny<Paciente>()))
                .ReturnsAsync(true);

            var resultado = await _pacienteService.CriarPacienteAsync(paciente);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("João Silva", resultado.Nome);
        }

        [Test]
        public void CriarPaciente_ComNomeVazio_DeveLancarExcecao()
        {
            var paciente = new Paciente
            {
                Nome = "",
                Email = "joao@example.com"
            };

            Assert.ThrowsAsync<ArgumentException>(async () => 
                await _pacienteService.CriarPacienteAsync(paciente));
        }
    }
}
```