using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Auth;
using TravelPlanner.Api.Models;
using TravelPlanner.Api.Security;

namespace TravelPlanner.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private static readonly HashSet<string> RolesPermitidas =
        new(StringComparer.OrdinalIgnoreCase) { "Viajante", "Guia" };

    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public AuthController(ApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto dto)
    {
        if (!RolesPermitidas.Contains(dto.Role))
            return BadRequest(new { message = "Role inválida. Cadastro público permitido apenas para Viajante ou Guia." });

        var role = await _context.Roles
            .FirstOrDefaultAsync(r => r.Nome.ToLower() == dto.Role.ToLower());

        if (role is null)
            return BadRequest(new { message = $"Role '{dto.Role}' não encontrada no sistema." });

        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { message = "E-mail já cadastrado." });

        var validationError = ValidarCamposPorRole(dto, role.Nome);
        if (validationError is not null)
            return BadRequest(new { message = validationError });

        var usuario = new Usuario
        {
            Nome = dto.Nome,
            Email = dto.Email,
            SenhaHash = PasswordHasher.Hash(dto.Senha),
            DataCriacao = DateTime.UtcNow,
            RoleId = role.Id
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        switch (role.Nome.ToLower())
        {
            case "viajante":
                _context.Viajantes.Add(new Viajante
                {
                    UsuarioId = usuario.Id,
                    PreferenciasViagem = dto.PreferenciasViagem
                });
                break;

            case "guia":
                _context.Guias.Add(new Guia
                {
                    UsuarioId = usuario.Id,
                    Especialidade = dto.Especialidade!,
                    Rating = 0
                });
                break;
        }

        await _context.SaveChangesAsync();

        usuario.Role = role;

        return Ok(new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(usuario),
            UsuarioId = usuario.Id,
            Email = usuario.Email,
            Role = role.Nome
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (usuario is null || !PasswordHasher.Verify(dto.Senha, usuario.SenhaHash))
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(usuario),
            UsuarioId = usuario.Id,
            Email = usuario.Email,
            Role = usuario.Role.Nome
        });
    }

    private static string? ValidarCamposPorRole(RegisterDto dto, string roleNome)
    {
        return roleNome.ToLower() switch
        {
            "guia" when string.IsNullOrWhiteSpace(dto.Especialidade)
                => "Especialidade é obrigatória para a role Guia.",
            _ => null
        };
    }
}
