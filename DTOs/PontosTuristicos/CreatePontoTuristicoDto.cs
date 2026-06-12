using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.PontosTuristicos;

public class CreatePontoTuristicoDto
{
    [Required]
    public int DestinoId { get; set; } = 1;

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}
