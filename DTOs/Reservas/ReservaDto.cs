namespace TravelPlanner.Api.DTOs.Reservas;

public class ReservaDto
{
    public int Id { get; set; }
    public string Hospedagem { get; set; } = string.Empty;
    public string? PacoteTitulo { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public int ViagemId { get; set; }
}
