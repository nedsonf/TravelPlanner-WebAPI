using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.Guias;

public class CreateAvaliacaoGuiaDto
{
    [Required]
    public int ReservaId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Nota { get; set; }

    [MaxLength(1000)]
    public string? Comentario { get; set; }
}
