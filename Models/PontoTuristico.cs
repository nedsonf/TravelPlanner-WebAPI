using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class PontoTuristico
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DestinoId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? OpenTripMapId { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    [ForeignKey(nameof(DestinoId))]
    public Destino Destino { get; set; } = null!;

    public ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();
}
