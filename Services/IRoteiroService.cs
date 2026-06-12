using TravelPlanner.Api.DTOs.Roteiro;

namespace TravelPlanner.Api.Services;

public interface IRoteiroService
{
    Task<RoteiroResumoDto?> ObterResumoAsync(int viagemId);
}
