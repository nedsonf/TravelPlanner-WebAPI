using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Public;

namespace TravelPlanner.Api.Controllers;

[ApiController]
[Route("api/public")]
public class CatalogoPublicoController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CatalogoPublicoController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("destinos")]
    public async Task<ActionResult<IEnumerable<DestinoPublicoDto>>> ListarDestinos()
    {
        var destinos = await _context.Destinos
            .OrderBy(d => d.Cidade)
            .Select(d => new DestinoPublicoDto
            {
                Id = d.Id,
                Nome = d.Cidade,
                Pais = d.Pais,
                Latitude = d.Latitude,
                Longitude = d.Longitude,
                TotalPontosTuristicos = d.PontosTuristicos.Count,
                PontosDestaque = d.PontosTuristicos
                    .OrderBy(p => p.Nome)
                    .Take(4)
                    .Select(p => p.Nome)
                    .ToList()
            })
            .ToListAsync();

        return Ok(destinos);
    }

    [HttpGet("hospedagens")]
    public async Task<ActionResult<IEnumerable<HospedagemPublicoDto>>> ListarHospedagens()
    {
        var hospedagens = await _context.Hospedagens
            .Include(h => h.Destino)
            .OrderBy(h => h.Nome)
            .Select(h => new HospedagemPublicoDto
            {
                Id = h.Id,
                Nome = h.Nome,
                Endereco = h.Endereco,
                DestinoId = h.DestinoId,
                DestinoNome = h.Destino.Cidade,
                ValorDiariaReferencia = h.Id == 1 ? 250.00m : 0m
            })
            .ToListAsync();

        return Ok(hospedagens);
    }

    [HttpGet("pontos-turisticos-populares")]
    public async Task<ActionResult<IEnumerable<PontoTuristicoPublicoDto>>> ListarPontosTuristicosPopulares()
    {
        var pontos = await _context.PontosTuristicos
            .Include(p => p.Destino)
            .OrderBy(p => p.Destino.Cidade)
            .ThenBy(p => p.Nome)
            .Select(p => new PontoTuristicoPublicoDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Categoria = p.Categoria,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                DestinoNome = p.Destino.Cidade
            })
            .ToListAsync();

        if (pontos.Count > 0)
            return Ok(pontos);

        return Ok(ObterPontosEstaticos());
    }

    private static List<PontoTuristicoPublicoDto> ObterPontosEstaticos() =>
    [
        new() { Id = 1, Nome = "Jalapão", Categoria = "Natureza", Latitude = -10.25, Longitude = -46.80 },
        new() { Id = 2, Nome = "Lago da Seda", Categoria = "Lago", Latitude = -10.30, Longitude = -48.20 },
        new() { Id = 3, Nome = "Catedral Metropolitana de Palmas", Categoria = "Religioso", Latitude = -10.1844, Longitude = -48.3336 }
    ];
}
