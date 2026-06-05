using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using FluentValidation;
using Consultorio.Application.Dtos.Paciente;
using Consultorio.Application.Results;
using Consultorio.Application.Exceptions;
using Consultorio.Domain.Entities;
using Consultorio.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consultorio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreatePacienteDto> _createValidator;
        private readonly IValidator<UpdatePacienteDto> _updateValidator;

        public PacienteController(
            IPacienteRepository repository,
            IMapper mapper,
            IValidator<CreatePacienteDto> createValidator,
            IValidator<UpdatePacienteDto> updateValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<PacienteResponseDto>>> GetById(Guid id)
        {
            var paciente = await _repository.GetByIdAsync(id);
            if (paciente == null)
                throw new NotFoundException(nameof(Paciente), id);

            var dto = _mapper.Map<PacienteResponseDto>(paciente);
            return Ok(Result<PacienteResponseDto>.Ok(dto));
        }

        [HttpGet]
        public async Task<ActionResult<Result<IEnumerable<PacienteResponseDto>>>> GetAll()
        {
            var pacientes = await _repository.GetAllAsync();
            var dtos = _mapper.Map<IEnumerable<PacienteResponseDto>>(pacientes);
            return Ok(Result<IEnumerable<PacienteResponseDto>>.Ok(dtos));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<PacienteResponseDto>>> Create([FromBody] CreatePacienteDto dto)
        {
            var validacao = await _createValidator.ValidateAsync(dto);
            if (!validacao.IsValid)
                throw new System.ComponentModel.DataAnnotations.ValidationException(string.Join(", ", validacao.Errors.Select(e => e.ErrorMessage)));

            var paciente = _mapper.Map<Paciente>(dto);
            await _repository.AddAsync(paciente);

            var resultado = _mapper.Map<PacienteResponseDto>(paciente);
            return CreatedAtAction(nameof(GetById), new { id = paciente.Id }, 
                Result<PacienteResponseDto>.Ok(resultado, "Paciente cadastrado com sucesso"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Recepcao")]
        public async Task<ActionResult<Result<PacienteResponseDto>>> Update(Guid id, [FromBody] UpdatePacienteDto dto)
        {
            var paciente = await _repository.GetByIdAsync(id);
            if (paciente == null)
                throw new NotFoundException(nameof(Paciente), id);

            dto.Id = id;
            var validacao = await _updateValidator.ValidateAsync(dto);
            if (!validacao.IsValid)
                throw new System.ComponentModel.DataAnnotations.ValidationException(string.Join(", ", validacao.Errors.Select(e => e.ErrorMessage)));

            _mapper.Map(dto, paciente);
            await _repository.UpdateAsync(paciente);

            var resultado = _mapper.Map<PacienteResponseDto>(paciente);
            return Ok(Result<PacienteResponseDto>.Ok(resultado, "Paciente atualizado com sucesso"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> Delete(Guid id)
        {
            var paciente = await _repository.GetByIdAsync(id);
            if (paciente == null)
                throw new NotFoundException(nameof(Paciente), id);

            await _repository.DeleteAsync(id);
            return Ok(Result.Ok("Paciente removido com sucesso"));
        }
    }
}
