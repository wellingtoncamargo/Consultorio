using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Consultorio.Application.Services;
using Consultorio.Application.Dtos.Auth;
using Consultorio.Application.Results;
using Consultorio.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace Consultorio.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public AuthController(ITokenService tokenService, IUsuarioRepository usuarioRepository, IPasswordHasherService passwordHasher)
        {
            _tokenService = tokenService;
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<Result<TokenResponseDto>>> Login([FromBody] LoginDto dto)
        {
            var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (usuario == null || !_passwordHasher.Verify(usuario.PasswordHash, dto.Senha))
            {
                // For backward compatibility: if no users exist, allow login to get token (development convenience)
                return Unauthorized(Result.Fail("Credenciais invalidas"));
            }

            var token = _tokenService.GenerateAccessToken(usuario.Id, usuario.Email, usuario.Role);
            var refresh = _tokenService.GenerateRefreshToken();

            var response = new TokenResponseDto
            {
                AccessToken = token,
                RefreshToken = refresh,
                ExpiresIn = DateTime.UtcNow.AddMinutes(15),
                Usuario = new UserInfoDto { Id = usuario.Id, Nome = usuario.Nome, Email = usuario.Email, Role = usuario.Role }
            };

            return Ok(Result<TokenResponseDto>.Ok(response));
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Result>> Register([FromBody] RegisterDto dto)
        {
            if (dto.Senha != dto.ConfirmSenha) return BadRequest(Result.Fail("Senha e confirmacao nao coincidem"));

            var existing = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existing != null) return Conflict(Result.Fail("Email ja cadastrado"));

            var usuario = new Consultorio.Domain.Entities.Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "Recepcao" : dto.Role,
                PasswordHash = _passwordHasher.HashPassword(dto.Senha)
            };

            await _usuarioRepository.AddAsync(usuario);
            return Ok(Result.Ok("Usuario registrado com sucesso"));
        }
    }
}