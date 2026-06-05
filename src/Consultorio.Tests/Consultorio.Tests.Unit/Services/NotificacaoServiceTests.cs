using System;
using System.Threading.Tasks;
using NUnit.Framework;

using Consultorio.Services;

namespace Consultorio.Tests.Unit.Services
{
    [TestFixture]
    public class NotificacaoServiceTests
    {
        [Test]
        public async Task EnviarEmailAgendamento_Should_ReturnFalse_When_SmtpNotConfigured()
        {
            var smtp = new ConfiguracaoSmtp { Host = "", Usuario = "" };
            var svc = new NotificacaoService(smtp);
            var ok = await svc.EnviarEmailAgendamentoAsync(new Consultorio.Domain.Entities.Agendamento { Paciente = new Consultorio.Domain.Entities.Paciente { Email = "x@x.com" } });
            Assert.That(ok, Is.False);
        }

        [Test]
        public async Task EnviarEmailAgendamento_Should_ReturnFalse_When_PacienteEmailMissing()
        {
            var smtp = new ConfiguracaoSmtp { Host = "smtp.example.com", Usuario = "u" , Senha = "p", Remetente = "r@r.com" };
            var svc = new NotificacaoService(smtp);
            var ok = await svc.EnviarEmailAgendamentoAsync(new Consultorio.Domain.Entities.Agendamento { Paciente = new Consultorio.Domain.Entities.Paciente { Email = "" } });
            Assert.That(ok, Is.False);
        }
    }
}
