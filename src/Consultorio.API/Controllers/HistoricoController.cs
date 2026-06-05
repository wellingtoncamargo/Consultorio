using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Consultorio.Domain.Repositories;
using Consultorio.Domain.Entities;
using Consultorio.Application.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Consultorio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HistoricoController : ControllerBase
    {
        private readonly IHistoricoRepository _repository;

        public HistoricoController(IHistoricoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<Historico>>> GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Historico nao encontrado"));
            return Ok(Result<Historico>.Ok(entity));
        }

        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<Historico>>>> GetAll()
        {
            var lista = await _repository.GetAllAsync();
            return Ok(Result<IEnumerable<Historico>>.Ok(lista));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<Historico>>> Create([FromBody] Historico dto)
        {
            await _repository.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, Result<Historico>.Ok(dto, "Historico cadastrado com sucesso"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<Historico>>> Update(Guid id, [FromBody] Historico dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Historico nao encontrado"));

            dto.Id = id;
            await _repository.UpdateAsync(dto);
            return Ok(Result<Historico>.Ok(dto, "Historico atualizado com sucesso"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Historico nao encontrado"));

            await _repository.DeleteAsync(id);
            return Ok(Result.Ok("Historico removido com sucesso"));
        }
    }
}