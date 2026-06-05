using System;
using System.Threading.Tasks;
using NUnit.Framework;

using Moq;
using Consultorio.Services;
using Consultorio.Domain.Entities;
using Consultorio.Domain.Repositories;

namespace Consultorio.Tests.Unit
{
    [TestFixture]
    public class PacienteServiceTests
    {
        private Mock<IPacienteRepository> _repoMock;
        private PacienteService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IPacienteRepository>();
            _service = new PacienteService(_repoMock.Object);
        }

        [Test]
        public async Task SalvarAsync_ShouldReturnFalse_When_NomeEmpty()
        {
            var paciente = new Paciente { Id = Guid.Empty, Nome = "", CPF = "123" };

            var res = await _service.SalvarAsync(paciente);

            Assert.That(res.ok, Is.False);
            Assert.That(res.erro.Contains("Nome"), Is.True);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Paciente>()), Times.Never);
        }

        [Test]
        public async Task SalvarAsync_ShouldReturnFalse_When_CPF_Duplicate()
        {
            var existing = new Paciente { Id = Guid.NewGuid(), CPF = "111" };
            _repoMock.Setup(r => r.GetByCPFAsync("111")).ReturnsAsync(existing);

            var paciente = new Paciente { Id = Guid.Empty, Nome = "Joao", CPF = "111" };
            var res = await _service.SalvarAsync(paciente);

            Assert.That(res.ok, Is.False);
            Assert.That(res.erro.Contains("CPF"), Is.True);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Paciente>()), Times.Never);
        }

        [Test]
        public async Task SalvarAsync_ShouldCreate_When_Valid_NewPaciente()
        {
            _repoMock.Setup(r => r.GetByCPFAsync(It.IsAny<string>())).ReturnsAsync((Paciente?)null);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Paciente>())).Returns(Task.CompletedTask).Verifiable();

            var paciente = new Paciente { Id = Guid.Empty, Nome = "Maria", CPF = "999" };
            var res = await _service.SalvarAsync(paciente);

            Assert.That(res.ok, Is.True);
            _repoMock.Verify(r => r.AddAsync(It.Is<Paciente>(p => p.Nome == "Maria")), Times.Once);
        }
    }
}
