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
    public class AgendamentoController : ControllerBase
    {
        private readonly IAgendamentoRepository _repository;

        public AgendamentoController(IAgendamentoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<Agendamento>>> GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Agendamento nao encontrado"));
            return Ok(Result<Agendamento>.Ok(entity));
        }

        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<Agendamento>>>> GetAll([FromQuery] DateTime? inicio = null, [FromQuery] DateTime? fim = null)
        {
            var lista = await _repository.GetComDetalhesAsync(inicio, fim);
            return Ok(Result<IEnumerable<Agendamento>>.Ok(lista));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<Agendamento>>> Create([FromBody] Agendamento dto)
        {
            await _repository.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, Result<Agendamento>.Ok(dto, "Agendamento cadastrado com sucesso"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<Agendamento>>> Update(Guid id, [FromBody] Agendamento dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Agendamento nao encontrado"));

            dto.Id = id;
            await _repository.UpdateAsync(dto);
            return Ok(Result<Agendamento>.Ok(dto, "Agendamento atualizado com sucesso"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Agendamento nao encontrado"));

            await _repository.DeleteAsync(id);
            return Ok(Result.Ok("Agendamento removido com sucesso"));
        }
    }
}