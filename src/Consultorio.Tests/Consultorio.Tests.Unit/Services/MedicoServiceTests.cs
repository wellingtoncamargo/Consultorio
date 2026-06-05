using System;
using System.Threading.Tasks;
using NUnit.Framework;

using Moq;
using Consultorio.Services;
using Consultorio.Domain.Entities;
using Consultorio.Domain.Repositories;

namespace Consultorio.Tests.Unit.Services
{
    [TestFixture]
    public class MedicoServiceTests
    {
        private Mock<IMedicoRepository> _repoMock;
        private MedicoService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IMedicoRepository>();
            _service = new MedicoService(_repoMock.Object);
        }

        [Test]
        public async Task SalvarAsync_ShouldReturnFalse_When_NomeEmpty()
        {
            var medico = new Medico { Id = Guid.Empty, Nome = "", CRM = "123" };
            var res = await _service.SalvarAsync(medico);
            Assert.That(res.ok, Is.False);
            Assert.That(res.erro.Contains("Nome"), Is.True);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Medico>()), Times.Never);
        }

        [Test]
        public async Task SalvarAsync_ShouldReturnFalse_When_CRMEmpty()
        {
            var medico = new Medico { Id = Guid.Empty, Nome = "Dr Test", CRM = "" };
            var res = await _service.SalvarAsync(medico);
            Assert.That(res.ok, Is.False);
            Assert.That(res.erro.Contains("CRM"), Is.True);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Medico>()), Times.Never);
        }

        [Test]
        public async Task SalvarAsync_ShouldReturnFalse_When_CRM_Duplicate()
        {
            var existing = new Medico { Id = Guid.NewGuid(), CRM = "CRM-1" };
            _repoMock.Setup(r => r.GetByCRMAsync("CRM-1")).ReturnsAsync(existing);

            var medico = new Medico { Id = Guid.Empty, Nome = "Dr X", CRM = "CRM-1" };
            var res = await _service.SalvarAsync(medico);

            Assert.That(res.ok, Is.False);
            Assert.That(res.erro.Contains("CRM"), Is.True);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Medico>()), Times.Never);
        }

        [Test]
        public async Task SalvarAsync_ShouldCreate_When_Valid_NewMedico()
        {
            _repoMock.Setup(r => r.GetByCRMAsync(It.IsAny<string>())).ReturnsAsync((Medico?)null);
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Medico>())).Returns(Task.CompletedTask).Verifiable();

            var medico = new Medico { Id = Guid.Empty, Nome = "Dr Valid", CRM = "CRM-999" };
            var res = await _service.SalvarAsync(medico);

            Assert.That(res.ok, Is.True);
            _repoMock.Verify(r => r.AddAsync(It.Is<Medico>(m => m.Nome == "Dr Valid")), Times.Once);
        }

        [Test]
        public async Task ExcluirAsync_ShouldReturnFalse_When_NotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Medico?)null);
            var ok = await _service.ExcluirAsync(Guid.NewGuid());
            Assert.That(ok, Is.False);
        }

        [Test]
        public async Task ExcluirAsync_ShouldSetAtivoFalse_When_Found()
        {
            var m = new Medico { Id = Guid.NewGuid(), Ativo = true };
            _repoMock.Setup(r => r.GetByIdAsync(m.Id)).ReturnsAsync(m);
            _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Medico>())).Returns(Task.CompletedTask).Verifiable();

            var ok = await _service.ExcluirAsync(m.Id);
            Assert.That(ok, Is.True);
            _repoMock.Verify(r => r.UpdateAsync(It.Is<Medico>(x => x.Ativo == false)), Times.Once);
        }
    }
}
