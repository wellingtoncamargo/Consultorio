using Consultorio.Data.Context;
using Consultorio.Domain.Entities;
using Consultorio.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Data.Repositories
{
    // ── Base Repository ────────────────────────────────────────────────────────
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly ConsultorioDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(ConsultorioDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync() =>
            await _dbSet.ToListAsync();

        public virtual async Task<T?> GetByIdAsync(Guid id) =>
            await _dbSet.FindAsync(id);

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }

    // ── Paciente Repository ────────────────────────────────────────────────────
    public class PacienteRepository : BaseRepository<Paciente>, IPacienteRepository
    {
        public PacienteRepository(ConsultorioDbContext context) : base(context) { }

        public async Task<IEnumerable<Paciente>> BuscarPorNomeAsync(string nome) =>
            await _dbSet.Where(p => p.Nome.ToLower().Contains(nome.ToLower()))
                        .OrderBy(p => p.Nome).ToListAsync();

        public async Task<IEnumerable<Paciente>> GetAtivosAsync() =>
            await _dbSet.Where(p => p.Ativo).OrderBy(p => p.Nome).ToListAsync();

        public async Task<Paciente?> GetByCPFAsync(string cpf) =>
            await _dbSet.FirstOrDefaultAsync(p => p.CPF == cpf);

        public async Task<IEnumerable<Paciente>> GetComAgendamentosAsync() =>
            await _dbSet.Include(p => p.Agendamentos).ToListAsync();

        public override async Task<IEnumerable<Paciente>> GetAllAsync() =>
            await _dbSet.OrderBy(p => p.Nome).ToListAsync();
    }

    // ── Medico Repository ──────────────────────────────────────────────────────
    public class MedicoRepository : BaseRepository<Medico>, IMedicoRepository
    {
        public MedicoRepository(ConsultorioDbContext context) : base(context) { }

        public async Task<IEnumerable<Medico>> BuscarPorNomeAsync(string nome) =>
            await _dbSet.Where(m => m.Nome.ToLower().Contains(nome.ToLower()))
                        .OrderBy(m => m.Nome).ToListAsync();

        public async Task<IEnumerable<Medico>> GetAtivosAsync() =>
            await _dbSet.Where(m => m.Ativo).OrderBy(m => m.Nome).ToListAsync();

        public async Task<Medico?> GetByCRMAsync(string crm) =>
            await _dbSet.FirstOrDefaultAsync(m => m.CRM == crm);

        public async Task<Medico?> GetComLocaisTrabalhoAsync(Guid id) =>
            await _dbSet.Include(m => m.LocaisTrabalho)
                        .FirstOrDefaultAsync(m => m.Id == id);

        public async Task<IEnumerable<Medico>> GetPorEspecialidadeAsync(string especialidade) =>
            await _dbSet.Where(m => m.Especialidade.ToLower().Contains(especialidade.ToLower()))
                        .OrderBy(m => m.Nome).ToListAsync();

        public override async Task<IEnumerable<Medico>> GetAllAsync() =>
            await _dbSet.OrderBy(m => m.Nome).ToListAsync();
    }

    // ── LocalTrabalho Repository ───────────────────────────────────────────────
    public class LocalTrabalhoRepository : BaseRepository<LocalTrabalho>, ILocalTrabalhoRepository
    {
        public LocalTrabalhoRepository(ConsultorioDbContext context) : base(context) { }

        public async Task<IEnumerable<LocalTrabalho>> GetPorMedicoAsync(Guid medicoId) =>
            await _dbSet.Where(l => l.MedicoId == medicoId)
                        .Include(l => l.Medico)
                        .OrderBy(l => l.Nome).ToListAsync();

        public async Task<IEnumerable<LocalTrabalho>> GetAtivosAsync() =>
            await _dbSet.Where(l => l.Ativo).OrderBy(l => l.Nome).ToListAsync();

        public async Task<IEnumerable<LocalTrabalho>> GetAtivosComMedicoAsync() =>
            await _dbSet.Where(l => l.Ativo)
                        .Include(l => l.Medico)
                        .OrderBy(l => l.Nome).ToListAsync();

        public override async Task<IEnumerable<LocalTrabalho>> GetAllAsync() =>
            await _dbSet.Include(l => l.Medico).OrderBy(l => l.Nome).ToListAsync();
    }

    // ── Agendamento Repository ─────────────────────────────────────────────────
    public class AgendamentoRepository : BaseRepository<Agendamento>, IAgendamentoRepository
    {
        public AgendamentoRepository(ConsultorioDbContext context) : base(context) { }

        public async Task<IEnumerable<Agendamento>> GetPorDataAsync(DateTime data) =>
            await _dbSet
                .Include(a => a.Paciente)
                .Include(a => a.Medico)
                .Include(a => a.LocalTrabalho)
                .Where(a => a.DataAgendamento.Date == data.Date)
                .OrderBy(a => a.HoraAgendamento).ToListAsync();

        public async Task<IEnumerable<Agendamento>> GetPorMedicoAsync(Guid medicoId) =>
            await _dbSet
                .Include(a => a.Paciente)
                .Include(a => a.LocalTrabalho)
                .Where(a => a.MedicoId == medicoId)
                .OrderByDescending(a => a.DataAgendamento)
                .ThenBy(a => a.HoraAgendamento).ToListAsync();

        public async Task<IEnumerable<Agendamento>> GetPorMedicoEDataAsync(Guid medicoId, DateTime data) =>
            await _dbSet
                .Include(a => a.Paciente)
                .Include(a => a.LocalTrabalho)
                .Where(a => a.MedicoId == medicoId && a.DataAgendamento.Date == data.Date)
                .OrderBy(a => a.HoraAgendamento).ToListAsync();

        public async Task<IEnumerable<Agendamento>> GetPorPacienteAsync(Guid pacienteId) =>
            await _dbSet
                .Include(a => a.Medico)
                .Include(a => a.LocalTrabalho)
                .Where(a => a.PacienteId == pacienteId)
                .OrderByDescending(a => a.DataAgendamento)
                .ThenBy(a => a.HoraAgendamento).ToListAsync();

        public async Task<IEnumerable<Agendamento>> GetPorPeriodoAsync(DateTime inicio, DateTime fim) =>
            await _dbSet
                .Include(a => a.Paciente)
                .Include(a => a.Medico)
                .Include(a => a.LocalTrabalho)
                .Where(a => a.DataAgendamento.Date >= inicio.Date && a.DataAgendamento.Date <= fim.Date)
                .OrderBy(a => a.DataAgendamento)
                .ThenBy(a => a.HoraAgendamento).ToListAsync();

        public async Task<IEnumerable<Agendamento>> GetComDetalhesAsync(DateTime? inicio = null, DateTime? fim = null)
        {
            var query = _dbSet
                .Include(a => a.Paciente)
                .Include(a => a.Medico)
                .Include(a => a.LocalTrabalho)
                .AsQueryable();

            if (inicio.HasValue) query = query.Where(a => a.DataAgendamento.Date >= inicio.Value.Date);
            if (fim.HasValue) query = query.Where(a => a.DataAgendamento.Date <= fim.Value.Date);

            return await query.OrderBy(a => a.DataAgendamento).ThenBy(a => a.HoraAgendamento).ToListAsync();
        }

        public async Task<bool> VerificarConflitoAsync(Guid medicoId, DateTime data, TimeSpan hora, int duracao, Guid? excluirId = null)
        {
            var novoInicio = hora;
            var novoFim = hora.Add(TimeSpan.FromMinutes(duracao));

            var agendamentos = await _dbSet
                .Where(a => a.MedicoId == medicoId &&
                            a.DataAgendamento.Date == data.Date &&
                            a.Status != Domain.Enums.StatusAgendamento.Cancelado &&
                            (excluirId == null || a.Id != excluirId))
                .ToListAsync();

            return agendamentos.Any(a =>
            {
                var existeInicio = a.HoraAgendamento;
                var existeFim = a.HoraAgendamento.Add(TimeSpan.FromMinutes(a.DuracaoMinutos));
                return novoInicio < existeFim && novoFim > existeInicio;
            });
        }

        public override async Task<IEnumerable<Agendamento>> GetAllAsync() =>
            await _dbSet
                .Include(a => a.Paciente)
                .Include(a => a.Medico)
                .Include(a => a.LocalTrabalho)
                .OrderByDescending(a => a.DataAgendamento)
                .ThenBy(a => a.HoraAgendamento).ToListAsync();
    }

    // ── Historico Repository ───────────────────────────────────────────────────
    public class HistoricoRepository : BaseRepository<Historico>, IHistoricoRepository
    {
        public HistoricoRepository(ConsultorioDbContext context) : base(context) { }

        public async Task<IEnumerable<Historico>> GetPorPacienteAsync(Guid pacienteId) =>
            await _dbSet
                .Include(h => h.Medico)
                .Where(h => h.PacienteId == pacienteId)
                .OrderByDescending(h => h.DataConsulta).ToListAsync();

        public async Task<IEnumerable<Historico>> GetPorMedicoAsync(Guid medicoId) =>
            await _dbSet
                .Include(h => h.Paciente)
                .Where(h => h.MedicoId == medicoId)
                .OrderByDescending(h => h.DataConsulta).ToListAsync();

        public async Task<IEnumerable<Historico>> GetComDetalhesAsync(Guid pacienteId) =>
            await _dbSet
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .Where(h => h.PacienteId == pacienteId)
                .OrderByDescending(h => h.DataConsulta).ToListAsync();

        public async Task<IEnumerable<Historico>> GetPorPeriodoAsync(DateTime inicio, DateTime fim) =>
            await _dbSet
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .Where(h => h.DataConsulta.Date >= inicio.Date && h.DataConsulta.Date <= fim.Date)
                .OrderByDescending(h => h.DataConsulta).ToListAsync();

        public override async Task<IEnumerable<Historico>> GetAllAsync() =>
            await _dbSet
                .Include(h => h.Paciente)
                .Include(h => h.Medico)
                .OrderByDescending(h => h.DataConsulta).ToListAsync();
    }
}
