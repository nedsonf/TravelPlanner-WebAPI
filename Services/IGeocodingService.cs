using TravelPlanner.Api.DTOs.ExternalApis;

namespace TravelPlanner.Api.Services;

public interface IGeocodingService
{
    Task<IReadOnlyList<CidadeEncontradaDto>> BuscarCidadesAsync(string nome);
}
