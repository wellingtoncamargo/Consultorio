using Consultorio.Domain.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Consultorio.Domain.Repositories
{
    public interface IUsuarioRepository : IRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> AnyAsync();
    }
}