using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Viajante
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [MaxLength(500)]
    public string? PreferenciasViagem { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

    public ContaFinanceira? ContaFinanceira { get; set; }
    public ICollection<Viagem> Viagens { get; set; } = new List<Viagem>();
}
