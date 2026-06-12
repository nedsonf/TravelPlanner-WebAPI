using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Destinos;
using TravelPlanner.Api.DTOs.ExternalApis;
using TravelPlanner.Api.Models;
using TravelPlanner.Api.Services;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/destinos")]
public class DestinoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IGeocodingService _geocodingService;

    public DestinoController(ApplicationDbContext context, IGeocodingService geocodingService)
    {
        _context = context;
        _geocodingService = geocodingService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DestinoDto>>> Listar()
    {
        var destinos = await _context.Destinos
            .OrderBy(d => d.Cidade)
            .Select(d => new DestinoDto
            {
                Id = d.Id,
                Cidade = d.Cidade,
                Pais = d.Pais,
                Latitude = d.Latitude,
                Longitude = d.Longitude
            })
            .ToListAsync();

        return Ok(destinos);
    }

    [HttpGet("buscar-cidades")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CidadeEncontradaDto>>> BuscarCidades([FromQuery] string nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 2)
            return BadRequest(new { message = "Informe ao menos 2 caracteres para a busca." });

        var cidades = await _geocodingService.BuscarCidadesAsync(nome.Trim());
        return Ok(cidades);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DestinoDto>> Criar([FromBody] CreateDestinoDto dto)
    {
        var jaExiste = await _context.Destinos.AnyAsync(d =>
            d.Cidade.ToLower() == dto.Cidade.ToLower() && d.Pais.ToLower() == dto.Pais.ToLower());

        if (jaExiste)
            return Conflict(new { message = $"Destino '{dto.Cidade}, {dto.Pais}' já está cadastrado." });

        var destino = new Destino
        {
            Cidade = dto.Cidade,
            Pais = dto.Pais,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        _context.Destinos.Add(destino);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Listar), new { id = destino.Id }, new DestinoDto
        {
            Id = destino.Id,
            Cidade = destino.Cidade,
            Pais = destino.Pais,
            Latitude = destino.Latitude,
            Longitude = destino.Longitude
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remover(int id)
    {
        var destino = await _context.Destinos.FindAsync(id);
        if (destino is null)
            return NotFound(new { message = $"Destino {id} não encontrado." });

        var possuiVinculos = await _context.Viagens.AnyAsync(v => v.DestinoId == id)
            || await _context.Hospedagens.AnyAsync(h => h.DestinoId == id)
            || await _context.PontosTuristicos.AnyAsync(p => p.DestinoId == id)
            || await _context.Pacotes.AnyAsync(p => p.DestinoId == id);

        if (possuiVinculos)
            return Conflict(new { message = "Destino possui viagens, hospedagens, pontos turísticos ou pacotes vinculados e não pode ser removido." });

        _context.Destinos.Remove(destino);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
