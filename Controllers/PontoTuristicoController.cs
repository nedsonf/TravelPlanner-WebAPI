using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.PontosTuristicos;
using TravelPlanner.Api.Models;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/pontos-turisticos")]
public class PontoTuristicoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PontoTuristicoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PontoTuristicoDto>>> Listar()
    {
        var pontos = await _context.PontosTuristicos
            .Include(p => p.Destino)
            .OrderBy(p => p.Nome)
            .Select(p => new PontoTuristicoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Categoria = p.Categoria,
                DestinoId = p.DestinoId,
                DestinoNome = p.Destino.Cidade,
                Latitude = p.Latitude,
                Longitude = p.Longitude
            })
            .ToListAsync();

        return Ok(pontos);
    }

    [HttpPost]
    [Authorize(Roles = "Guia")]
    public async Task<ActionResult<PontoTuristicoDto>> Criar([FromBody] CreatePontoTuristicoDto dto)
    {
        var destino = await _context.Destinos.FindAsync(dto.DestinoId);
        if (destino is null)
            return BadRequest(new { message = $"Destino {dto.DestinoId} não encontrado." });

        var ponto = new PontoTuristico
        {
            DestinoId = dto.DestinoId,
            Nome = dto.Nome,
            Categoria = dto.Categoria,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        _context.PontosTuristicos.Add(ponto);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Listar), new { id = ponto.Id }, new PontoTuristicoDto
        {
            Id = ponto.Id,
            Nome = ponto.Nome,
            Categoria = ponto.Categoria,
            DestinoId = ponto.DestinoId,
            DestinoNome = destino.Cidade,
            Latitude = ponto.Latitude,
            Longitude = ponto.Longitude
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Guia")]
    public async Task<IActionResult> Remover(int id)
    {
        var ponto = await _context.PontosTuristicos.FindAsync(id);
        if (ponto is null)
            return NotFound(new { message = $"Ponto turístico {id} não encontrado." });

        var possuiAvaliacoes = await _context.Avaliacoes.AnyAsync(a => a.PontoTuristicoId == id);
        if (possuiAvaliacoes)
            return Conflict(new { message = "Ponto turístico possui avaliações vinculadas e não pode ser removido." });

        _context.PontosTuristicos.Remove(ponto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
