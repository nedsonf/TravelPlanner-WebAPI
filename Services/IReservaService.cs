using TravelPlanner.Api.DTOs.Reservas;

namespace TravelPlanner.Api.Services;

public interface IReservaService
{
    Task<ReservaCriadaDto?> CriarReservaComDespesaAsync(CreateReservaDto dto);
}
