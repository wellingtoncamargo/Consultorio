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
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoRepository _repository;

        public MedicoController(IMedicoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<Medico>>> GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Medico nao encontrado"));
            return Ok(Result<Medico>.Ok(entity));
        }

        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<Medico>>>> GetAll()
        {
            var lista = await _repository.GetAllAsync();
            return Ok(Result<IEnumerable<Medico>>.Ok(lista));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<Medico>>> Create([FromBody] Medico dto)
        {
            await _repository.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, Result<Medico>.Ok(dto, "Medico cadastrado com sucesso"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<Medico>>> Update(Guid id, [FromBody] Medico dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Medico nao encontrado"));

            // Simple field updates (copy fields)
            dto.Id = id;
            await _repository.UpdateAsync(dto);
            return Ok(Result<Medico>.Ok(dto, "Medico atualizado com sucesso"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Medico nao encontrado"));

            await _repository.DeleteAsync(id);
            return Ok(Result.Ok("Medico removido com sucesso"));
        }
    }
}