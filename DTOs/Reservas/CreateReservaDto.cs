using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.Reservas;

public class CreateReservaDto
{
    [Required]
    public int ViagemId { get; set; }

    [Required]
    public int HospedagemId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal ValorTotal { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Pendente";
}
