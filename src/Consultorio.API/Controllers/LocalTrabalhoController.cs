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
    public class LocalTrabalhoController : ControllerBase
    {
        private readonly ILocalTrabalhoRepository _repository;

        public LocalTrabalhoController(ILocalTrabalhoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<LocalTrabalho>>> GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Local de trabalho nao encontrado"));
            return Ok(Result<LocalTrabalho>.Ok(entity));
        }

        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<LocalTrabalho>>>> GetAll()
        {
            var lista = await _repository.GetAllAsync();
            return Ok(Result<IEnumerable<LocalTrabalho>>.Ok(lista));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<LocalTrabalho>>> Create([FromBody] LocalTrabalho dto)
        {
            await _repository.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, Result<LocalTrabalho>.Ok(dto, "Local criado com sucesso"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<LocalTrabalho>>> Update(Guid id, [FromBody] LocalTrabalho dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Local de trabalho nao encontrado"));

            dto.Id = id;
            await _repository.UpdateAsync(dto);
            return Ok(Result<LocalTrabalho>.Ok(dto, "Local atualizado com sucesso"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound(Result.Fail("Local de trabalho nao encontrado"));

            await _repository.DeleteAsync(id);
            return Ok(Result.Ok("Local removido com sucesso"));
        }
    }
}