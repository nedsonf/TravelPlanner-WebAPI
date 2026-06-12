using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.Roteiro;

public class CreateRoteiroDiarioDto
{
    [Required]
    public int ViagemId { get; set; } = 1;

    [Required]
    public DateTime Data { get; set; }

    [Required]
    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;
}
