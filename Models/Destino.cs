using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.Models;

public class Destino
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Pais { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    public ICollection<Viagem> Viagens { get; set; } = new List<Viagem>();
    public ICollection<PontoTuristico> PontosTuristicos { get; set; } = new List<PontoTuristico>();
    public ICollection<Hospedagem> Hospedagens { get; set; } = new List<Hospedagem>();
}
