using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.Viagens;

public class CreateViagemDto
{
    [Required]
    public int DestinoId { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    [Required]
    public DateTime DataFim { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "Planejada";
}
