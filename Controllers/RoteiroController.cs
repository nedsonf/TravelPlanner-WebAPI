using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Roteiro;
using TravelPlanner.Api.Models;
using TravelPlanner.Api.Services;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/roteiro")]
public class RoteiroController : ControllerBase
{
    private readonly IRoteiroService _roteiroService;
    private readonly ApplicationDbContext _context;

    public RoteiroController(IRoteiroService roteiroService, ApplicationDbContext context)
    {
        _roteiroService = roteiroService;
        _context = context;
    }

    [HttpGet("{viagemId}/resumo")]
    public async Task<ActionResult<RoteiroResumoDto>> ObterResumo(int viagemId)
    {
        var resumo = await _roteiroService.ObterResumoAsync(viagemId);

        if (resumo is null)
            return NotFound(new { message = $"Viagem {viagemId} não encontrada." });

        return Ok(resumo);
    }

    [HttpGet("{viagemId}/diarios")]
    public async Task<ActionResult<IEnumerable<RoteiroDiarioDto>>> ListarDiarios(int viagemId)
    {
        var diarios = await _context.RoteirosDiarios
            .Where(r => r.ViagemId == viagemId)
            .OrderBy(r => r.Data)
            .Select(r => new RoteiroDiarioDto
            {
                Id = r.Id,
                ViagemId = r.ViagemId,
                Data = r.Data,
                Descricao = r.Descricao
            })
            .ToListAsync();

        return Ok(diarios);
    }

    [HttpPost("diarios")]
    [Authorize(Roles = "Guia")]
    public async Task<ActionResult<RoteiroDiarioDto>> CriarDiario([FromBody] CreateRoteiroDiarioDto dto)
    {
        var viagemExiste = await _context.Viagens.AnyAsync(v => v.Id == dto.ViagemId);
        if (!viagemExiste)
            return BadRequest(new { message = $"Viagem {dto.ViagemId} não encontrada." });

        var roteiro = new RoteiroDiario
        {
            ViagemId = dto.ViagemId,
            Data = dto.Data,
            Descricao = dto.Descricao
        };

        _context.RoteirosDiarios.Add(roteiro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ListarDiarios), new { viagemId = roteiro.ViagemId }, new RoteiroDiarioDto
        {
            Id = roteiro.Id,
            ViagemId = roteiro.ViagemId,
            Data = roteiro.Data,
            Descricao = roteiro.Descricao
        });
    }

    [HttpDelete("diarios/{id}")]
    [Authorize(Roles = "Guia")]
    public async Task<IActionResult> RemoverDiario(int id)
    {
        var roteiro = await _context.RoteirosDiarios.FindAsync(id);
        if (roteiro is null)
            return NotFound(new { message = $"Roteiro diário {id} não encontrado." });

        _context.RoteirosDiarios.Remove(roteiro);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
