using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.ExternalApis;
using TravelPlanner.Api.DTOs.Roteiro;

namespace TravelPlanner.Api.Services;

public class RoteiroService : IRoteiroService
{
    private readonly ApplicationDbContext _context;
    private readonly IOpenWeatherService _openWeatherService;
    private readonly ILogger<RoteiroService> _logger;

    public RoteiroService(
        ApplicationDbContext context,
        IOpenWeatherService openWeatherService,
        ILogger<RoteiroService> logger)
    {
        _context = context;
        _openWeatherService = openWeatherService;
        _logger = logger;
    }

    public async Task<RoteiroResumoDto?> ObterResumoAsync(int viagemId)
    {
        var viagem = await _context.Viagens
            .Include(v => v.Destino)
            .FirstOrDefaultAsync(v => v.Id == viagemId);

        if (viagem is null)
        {
            _logger.LogWarning("Viagem {ViagemId} não encontrada para resumo do roteiro.", viagemId);
            return null;
        }

        var destino = viagem.Destino;

        var previsao = await _openWeatherService.GetFiveDayForecastAsync(
            destino.Latitude, destino.Longitude) ?? [];

        var pontosSugeridos = await _context.PontosTuristicos
            .Where(p => p.DestinoId == destino.Id)
            .OrderBy(p => p.Nome)
            .Take(3)
            .Select(p => new TouristPointDto(p.Nome, p.Categoria, p.Latitude, p.Longitude))
            .ToListAsync();

        return new RoteiroResumoDto
        {
            ViagemId = viagem.Id,
            Status = viagem.Status,
            DataInicio = viagem.DataInicio,
            DataFim = viagem.DataFim,
            Destino = new DestinoResumoDto
            {
                Id = destino.Id,
                Cidade = destino.Cidade,
                Pais = destino.Pais,
                Latitude = destino.Latitude,
                Longitude = destino.Longitude
            },
            PrevisaoClima = previsao,
            PontosTuristicosSugeridos = pontosSugeridos
        };
    }
}
