using TravelPlanner.Api.DTOs.Pacotes;

namespace TravelPlanner.Api.Services;

public interface IPacoteService
{
    Task<PacoteReservadoDto?> ReservarPacoteAsync(int pacoteId, int usuarioId);
}
