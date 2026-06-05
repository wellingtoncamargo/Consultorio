using Consultorio.Domain.Entities;
using Consultorio.Domain.Enums;
using Consultorio.Domain.Repositories;
using System.Net;
using System.Net.Mail;

namespace Consultorio.Services
{
    // ── Paciente Service ───────────────────────────────────────────────────────
    public class PacienteService
    {
        private readonly IPacienteRepository _repo;
        public PacienteService(IPacienteRepository repo) => _repo = repo;

        public Task<IEnumerable<Paciente>> GetAllAsync() => _repo.GetAtivosAsync();
        public Task<IEnumerable<Paciente>> BuscarAsync(string termo) => _repo.BuscarPorNomeAsync(termo);
        public Task<Paciente?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public async Task<(bool ok, string erro)> SalvarAsync(Paciente p)
        {
            if (string.IsNullOrWhiteSpace(p.Nome)) return (false, "Nome é obrigatório.");
            if (!string.IsNullOrWhiteSpace(p.CPF))
            {
                var existente = await _repo.GetByCPFAsync(p.CPF);
                if (existente != null && existente.Id != p.Id)
                    return (false, "Já existe um paciente com esse CPF.");
            }
            p.DataAtualizacao = DateTime.Now;
            if (p.Id == Guid.Empty) { p.Id = Guid.NewGuid(); p.DataCriacao = DateTime.Now; await _repo.AddAsync(p); }
            else await _repo.UpdateAsync(p);
            return (true, string.Empty);
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return false;
            p.Ativo = false;
            p.DataAtualizacao = DateTime.Now;
            await _repo.UpdateAsync(p);
            return true;
        }
    }

    // ── Medico Service ─────────────────────────────────────────────────────────
    public class MedicoService
    {
        private readonly IMedicoRepository _repo;
        public MedicoService(IMedicoRepository repo) => _repo = repo;

        public Task<IEnumerable<Medico>> GetAllAsync() => _repo.GetAtivosAsync();
        public Task<IEnumerable<Medico>> BuscarAsync(string termo) => _repo.BuscarPorNomeAsync(termo);
        public Task<Medico?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);
        public Task<Medico?> GetComLocaisAsync(Guid id) => _repo.GetComLocaisTrabalhoAsync(id);

        public async Task<(bool ok, string erro)> SalvarAsync(Medico m)
        {
            if (string.IsNullOrWhiteSpace(m.Nome)) return (false, "Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(m.CRM)) return (false, "CRM é obrigatório.");
            var existente = await _repo.GetByCRMAsync(m.CRM);
            if (existente != null && existente.Id != m.Id)
                return (false, "Já existe um médico com esse CRM.");
            m.DataAtualizacao = DateTime.Now;
            if (m.Id == Guid.Empty) { m.Id = Guid.NewGuid(); m.DataCriacao = DateTime.Now; await _repo.AddAsync(m); }
            else await _repo.UpdateAsync(m);
            return (true, string.Empty);
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var m = await _repo.GetByIdAsync(id);
            if (m == null) return false;
            m.Ativo = false;
            m.DataAtualizacao = DateTime.Now;
            await _repo.UpdateAsync(m);
            return true;
        }
    }

    // ── LocalTrabalho Service ──────────────────────────────────────────────────
    public class LocalTrabalhoService
    {
        private readonly ILocalTrabalhoRepository _repo;
        public LocalTrabalhoService(ILocalTrabalhoRepository repo) => _repo = repo;

        public Task<IEnumerable<LocalTrabalho>> GetAllAsync() => _repo.GetAtivosComMedicoAsync();
        public Task<IEnumerable<LocalTrabalho>> GetPorMedicoAsync(Guid medicoId) => _repo.GetPorMedicoAsync(medicoId);
        public Task<LocalTrabalho?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public async Task<(bool ok, string erro)> SalvarAsync(LocalTrabalho l)
        {
            if (string.IsNullOrWhiteSpace(l.Nome)) return (false, "Nome do local é obrigatório.");
            if (l.MedicoId == Guid.Empty) return (false, "Selecione um médico.");
            l.DataAtualizacao = DateTime.Now;
            if (l.Id == Guid.Empty) { l.Id = Guid.NewGuid(); l.DataCriacao = DateTime.Now; await _repo.AddAsync(l); }
            else await _repo.UpdateAsync(l);
            return (true, string.Empty);
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var l = await _repo.GetByIdAsync(id);
            if (l == null) return false;
            l.Ativo = false;
            l.DataAtualizacao = DateTime.Now;
            await _repo.UpdateAsync(l);
            return true;
        }
    }

    // ── Agendamento Service ────────────────────────────────────────────────────
    public class AgendamentoService
    {
        private readonly IAgendamentoRepository _repo;
        private readonly NotificacaoService _notificacao;

        public AgendamentoService(IAgendamentoRepository repo, NotificacaoService notificacao)
        {
            _repo = repo;
            _notificacao = notificacao;
        }

        public Task<IEnumerable<Agendamento>> GetPorDataAsync(DateTime data) => _repo.GetPorDataAsync(data);
        public Task<IEnumerable<Agendamento>> GetPorMedicoAsync(Guid medicoId) => _repo.GetPorMedicoAsync(medicoId);
        public Task<IEnumerable<Agendamento>> GetPorPeriodoAsync(DateTime inicio, DateTime fim) => _repo.GetPorPeriodoAsync(inicio, fim);
        public Task<IEnumerable<Agendamento>> GetPorMedicoEDataAsync(Guid medicoId, DateTime data) => _repo.GetPorMedicoEDataAsync(medicoId, data);
        public Task<IEnumerable<Agendamento>> GetPorPacienteAsync(Guid pacienteId) => _repo.GetPorPacienteAsync(pacienteId);
        public Task<Agendamento?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public async Task<(bool ok, string erro)> SalvarAsync(Agendamento a)
        {
            if (a.PacienteId == Guid.Empty) return (false, "Selecione um paciente.");
            if (a.MedicoId == Guid.Empty) return (false, "Selecione um médico.");
            if (a.LocalTrabalhoId == Guid.Empty) return (false, "Selecione um local de atendimento.");
            if (a.DataAgendamento == default) return (false, "Data inválida.");

            var conflito = await _repo.VerificarConflitoAsync(a.MedicoId, a.DataAgendamento, a.HoraAgendamento, a.DuracaoMinutos, a.Id == Guid.Empty ? null : a.Id);
            if (conflito) return (false, "Já existe um agendamento para esse médico nesse horário.");

            a.DataAtualizacao = DateTime.Now;
            if (a.Id == Guid.Empty) { a.Id = Guid.NewGuid(); a.DataCriacao = DateTime.Now; await _repo.AddAsync(a); }
            else await _repo.UpdateAsync(a);
            return (true, string.Empty);
        }

        public async Task<bool> CancelarAsync(Guid id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null || !a.PodeCancelar) return false;
            a.Status = StatusAgendamento.Cancelado;
            a.DataAtualizacao = DateTime.Now;
            await _repo.UpdateAsync(a);
            return true;
        }

        public async Task<bool> ConfirmarAsync(Guid id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return false;
            a.Status = StatusAgendamento.Confirmado;
            a.DataAtualizacao = DateTime.Now;
            await _repo.UpdateAsync(a);
            return true;
        }

        public async Task<bool> RealizarAsync(Guid id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a == null) return false;
            a.Status = StatusAgendamento.Realizado;
            a.DataAtualizacao = DateTime.Now;
            await _repo.UpdateAsync(a);
            return true;
        }

        //public async Task<(bool ok, string erro)> EnviarNotificacaoAsync(Guid id, TipoNotificacao tipo)
        //{
        //    var a = await _repo.GetByIdAsync(id);
        //    if (a == null) return (false, "Agendamento não encontrado.");

        //    if (tipo == TipoNotificacao.Email || tipo == TipoNotificacao.Ambos)
        //    {
        //        var ok = await _notificacao.EnviarEmailAgendamentoAsync(a);
        //        if (ok) { a.NotificadoPorEmail = true; a.DataAtualizacao = DateTime.Now; await _repo.UpdateAsync(a); }
        //    }

        //    if (tipo == TipoNotificacao.WhatsApp || tipo == TipoNotificacao.Ambos)
        //    {
        //        _notificacao.AbrirWhatsAppAgendamento(a);
        //        a.NotificadoPorWhatsApp = true;
        //        a.DataAtualizacao = DateTime.Now;
        //        await _repo.UpdateAsync(a);
        //    }

        //    return (true, string.Empty);
        //}
    }

    // ── Historico Service ──────────────────────────────────────────────────────
    public class HistoricoService
    {
        private readonly IHistoricoRepository _repo;
        public HistoricoService(IHistoricoRepository repo) => _repo = repo;

        public Task<IEnumerable<Historico>> GetPorPacienteAsync(Guid pacienteId) => _repo.GetComDetalhesAsync(pacienteId);
        public Task<IEnumerable<Historico>> GetPorMedicoAsync(Guid medicoId) => _repo.GetPorMedicoAsync(medicoId);
        public Task<Historico?> GetByIdAsync(Guid id) => _repo.GetByIdAsync(id);

        public async Task<(bool ok, string erro)> SalvarAsync(Historico h)
        {
            if (h.PacienteId == Guid.Empty) return (false, "Selecione um paciente.");
            if (h.MedicoId == Guid.Empty) return (false, "Selecione um médico.");
            if (h.DataConsulta == default) return (false, "Data inválida.");
            h.DataAtualizacao = DateTime.Now;
            if (h.Id == Guid.Empty) { h.Id = Guid.NewGuid(); h.DataCriacao = DateTime.Now; await _repo.AddAsync(h); }
            else await _repo.UpdateAsync(h);
            return (true, string.Empty);
        }

        public async Task<bool> ExcluirAsync(Guid id)
        {
            var h = await _repo.GetByIdAsync(id);
            if (h == null) return false;
            await _repo.DeleteAsync(id);
            return true;
        }
    }

    // ── Notificacao Service ────────────────────────────────────────────────────
    public class NotificacaoService
    {
        private readonly ConfiguracaoSmtp _smtp;

        public NotificacaoService(ConfiguracaoSmtp smtp) => _smtp = smtp;

        public async Task<bool> EnviarEmailAgendamentoAsync(Agendamento agendamento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(agendamento.Paciente?.Email)) return false;
                if (!_smtp.Configurado) return false;

                var corpo = GerarCorpoEmail(agendamento);
                using var client = new SmtpClient(_smtp.Host, _smtp.Porta)
                {
                    Credentials = new NetworkCredential(_smtp.Usuario, _smtp.Senha),
                    EnableSsl = _smtp.UsarSsl
                };
                var msg = new MailMessage(_smtp.Remetente, agendamento.Paciente.Email)
                {
                    Subject = $"Confirmação de Consulta - {agendamento.DataAgendamento:dd/MM/yyyy}",
                    Body = corpo,
                    IsBodyHtml = true
                };
                await client.SendMailAsync(msg);
                return true;
            }
            catch { return false; }
        }

        //public void AbrirWhatsAppAgendamento(Agendamento a)
        //{
        //    var celular = a.Paciente?.Celular?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
        //    if (string.IsNullOrWhiteSpace(celular)) return;
        //    if (!celular.StartsWith("55")) celular = "55" + celular;
        //    var msg = GerarMensagemWhatsApp(a);
        //    var url = $"https://wa.me/{celular}?text={Uri.EscapeDataString(msg)}";
        //    try { Microsoft.Maui.ApplicationModel.Launcher.Default.OpenAsync(new Uri(url)); } catch { }
        //}

        private string GerarMensagemWhatsApp(Agendamento a) =>
            $"Olá {a.Paciente?.Nome}! 👋\n\n" +
            $"Sua consulta está confirmada:\n" +
            $"📅 Data: {a.DataAgendamento:dd/MM/yyyy}\n" +
            $"⏰ Horário: {a.HoraAgendamento:hh\\:mm}\n" +
            $"👨‍⚕️ Médico: Dr(a). {a.Medico?.Nome}\n" +
            $"🏥 Local: {a.LocalTrabalho?.Nome}\n" +
            $"📍 Endereço: {a.LocalTrabalho?.EnderecoCompleto}\n\n" +
            $"Em caso de dúvidas, entre em contato conosco.";

        private string GerarCorpoEmail(Agendamento a) => $"""
            <!DOCTYPE html><html><body style="font-family:Arial,sans-serif;max-width:600px;margin:0 auto;padding:20px;">
            <div style="background:#1a73e8;padding:30px;border-radius:10px 10px 0 0;text-align:center;">
              <h1 style="color:white;margin:0;">Confirmação de Consulta</h1>
            </div>
            <div style="background:#f9f9f9;padding:30px;border-radius:0 0 10px 10px;">
              <p>Olá, <strong>{a.Paciente?.Nome}</strong>!</p>
              <p>Sua consulta está agendada com os seguintes detalhes:</p>
              <table style="width:100%;border-collapse:collapse;">
                <tr><td style="padding:8px;border-bottom:1px solid #ddd;"><strong>📅 Data:</strong></td><td style="padding:8px;border-bottom:1px solid #ddd;">{a.DataAgendamento:dd/MM/yyyy}</td></tr>
                <tr><td style="padding:8px;border-bottom:1px solid #ddd;"><strong>⏰ Horário:</strong></td><td style="padding:8px;border-bottom:1px solid #ddd;">{a.HoraAgendamento:hh\\:mm}</td></tr>
                <tr><td style="padding:8px;border-bottom:1px solid #ddd;"><strong>👨‍⚕️ Médico:</strong></td><td style="padding:8px;border-bottom:1px solid #ddd;">Dr(a). {a.Medico?.Nome} - {a.Medico?.Especialidade}</td></tr>
                <tr><td style="padding:8px;border-bottom:1px solid #ddd;"><strong>🏥 Local:</strong></td><td style="padding:8px;border-bottom:1px solid #ddd;">{a.LocalTrabalho?.Nome}</td></tr>
                <tr><td style="padding:8px;"><strong>📍 Endereço:</strong></td><td style="padding:8px;">{a.LocalTrabalho?.EnderecoCompleto}</td></tr>
              </table>
              {(string.IsNullOrWhiteSpace(a.Observacoes) ? "" : $"<p><strong>Observações:</strong> {a.Observacoes}</p>")}
              <p style="color:#666;font-size:12px;margin-top:20px;">Por favor, chegue com 15 minutos de antecedência.</p>
            </div></body></html>
            """;
    }

    // ── Configuracao SMTP ──────────────────────────────────────────────────────
    public class ConfiguracaoSmtp
    {
        public string Host { get; set; } = string.Empty;
        public int Porta { get; set; } = 587;
        public string Usuario { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Remetente { get; set; } = string.Empty;
        public string NomeRemetente { get; set; } = "Consultório";
        public bool UsarSsl { get; set; } = true;
        public bool Configurado => !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(Usuario);
    }
}