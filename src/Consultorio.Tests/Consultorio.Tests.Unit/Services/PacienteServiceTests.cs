using Moq;
using System;
using System.Threading.Tasks;
using Consultorio.Domain.Entities;
using Consultorio.Services;
using NUnit.Framework;
using Consultorio.Domain.Repositories;

namespace Consultorio.Tests.Unit.Services
{
    [TestFixture]
    public class PacienteServiceTests
    {
        private Mock<IPacienteRepository> _pacienteRepositoryMock = null!;
        private PacienteService _pacienteService = null!;

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

            _pacienteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Paciente>())).Returns(Task.CompletedTask);

            var resultado = await _pacienteService.SalvarAsync(paciente);

            Assert.That(resultado.ok, Is.True);
            Assert.That(paciente.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task CriarPaciente_ComNomeVazio_DeveLancarExcecao()
        {
            var paciente = new Paciente
            {
                Nome = "",
                Email = "joao@example.com"
            };

            var resultado = await _pacienteService.SalvarAsync(paciente);
            Assert.That(resultado.ok, Is.False);
            Assert.That(resultado.erro, Does.Contain("Nome é obrigatório"));
        }
    }
}

