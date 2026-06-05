using Consultorio.Domain.Entities;

namespace Consultorio.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<int> SaveChangesAsync();
    }

    public interface IPacienteRepository : IRepository<Paciente>
    {
        Task<IEnumerable<Paciente>> BuscarPorNomeAsync(string nome);
        Task<IEnumerable<Paciente>> GetAtivosAsync();
        Task<Paciente?> GetByCPFAsync(string cpf);
        Task<IEnumerable<Paciente>> GetComAgendamentosAsync();
    }

    public interface IMedicoRepository : IRepository<Medico>
    {
        Task<IEnumerable<Medico>> BuscarPorNomeAsync(string nome);
        Task<IEnumerable<Medico>> GetAtivosAsync();
        Task<Medico?> GetByCRMAsync(string crm);
        Task<Medico?> GetComLocaisTrabalhoAsync(Guid id);
        Task<IEnumerable<Medico>> GetPorEspecialidadeAsync(string especialidade);
    }

    public interface ILocalTrabalhoRepository : IRepository<LocalTrabalho>
    {
        Task<IEnumerable<LocalTrabalho>> GetPorMedicoAsync(Guid medicoId);
        Task<IEnumerable<LocalTrabalho>> GetAtivosAsync();
        Task<IEnumerable<LocalTrabalho>> GetAtivosComMedicoAsync();
    }

    public interface IAgendamentoRepository : IRepository<Agendamento>
    {
        Task<IEnumerable<Agendamento>> GetPorDataAsync(DateTime data);
        Task<IEnumerable<Agendamento>> GetPorMedicoAsync(Guid medicoId);
        Task<IEnumerable<Agendamento>> GetPorMedicoEDataAsync(Guid medicoId, DateTime data);
        Task<IEnumerable<Agendamento>> GetPorPacienteAsync(Guid pacienteId);
        Task<IEnumerable<Agendamento>> GetPorPeriodoAsync(DateTime inicio, DateTime fim);
        Task<IEnumerable<Agendamento>> GetComDetalhesAsync(DateTime? inicio = null, DateTime? fim = null);
        Task<bool> VerificarConflitoAsync(Guid medicoId, DateTime data, TimeSpan hora, int duracao, Guid? excluirId = null);
    }

    public interface IHistoricoRepository : IRepository<Historico>
    {
        Task<IEnumerable<Historico>> GetPorPacienteAsync(Guid pacienteId);
        Task<IEnumerable<Historico>> GetPorMedicoAsync(Guid medicoId);
        Task<IEnumerable<Historico>> GetComDetalhesAsync(Guid pacienteId);
        Task<IEnumerable<Historico>> GetPorPeriodoAsync(DateTime inicio, DateTime fim);
    }
}
