using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

using Consultorio.Services;
using Consultorio.Domain.Entities;
using Consultorio.Domain.Repositories;

namespace Consultorio.Tests.Unit.Services
{
    [TestFixture]
    public class AgendamentoServiceTests
    {
        private Mock<IAgendamentoRepository> _repoMock;
        private Mock<NotificacaoService> _notMock;
        private AgendamentoService _service;

       [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IAgendamentoRepository>();
            _notMock = new Mock<NotificacaoService>(null);
            _service = new AgendamentoService(_repoMock.Object, _notMock.Object);
        }

       [Test]
        public async Task SalvarAsync_Should_ReturnFalse_When_Missing_Ids()
        {
            var a = new Agendamento { PacienteId = Guid.Empty, MedicoId = Guid.Empty, LocalTrabalhoId = Guid.Empty };
            var res = await _service.SalvarAsync(a);
            Assert.That(res.ok, Is.False);
        }

       [Test]
        public async Task SalvarAsync_Should_ReturnFalse_When_Conflict()
        {
            var a = new Agendamento { Id = Guid.Empty, PacienteId = Guid.NewGuid(), MedicoId = Guid.NewGuid(), LocalTrabalhoId = Guid.NewGuid(), DataAgendamento = DateTime.Today, HoraAgendamento = TimeSpan.FromHours(9), DuracaoMinutos = 30 };
            _repoMock.Setup(r => r.VerificarConflitoAsync(a.MedicoId, a.DataAgendamento, a.HoraAgendamento, a.DuracaoMinutos, null)).ReturnsAsync(true);

           var res = await _service.SalvarAsync(a);
            Assert.That(res.ok, Is.False);
        }

       [Test]
        public async Task SalvarAsync_Should_Create_When_Valid()
        {
            var a = new Agendamento { Id = Guid.Empty, PacienteId = Guid.NewGuid(), MedicoId = Guid.NewGuid(), LocalTrabalhoId = Guid.NewGuid(), DataAgendamento = DateTime.Today, HoraAgendamento = TimeSpan.FromHours(11), DuracaoMinutos = 30 };
            _repoMock.Setup(r => r.VerificarConflitoAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<int>(), null)).ReturnsAsync(false);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Agendamento>())).Returns(Task.CompletedTask).Verifiable();

           var res = await _service.SalvarAsync(a);
            Assert.That(res.ok, Is.True);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Agendamento>()), Times.Once);
        }
    }
}
