using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Guia
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Especialidade { get; set; } = string.Empty;

    [Range(0, 5)]
    public decimal Rating { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

    public ICollection<AvaliacaoGuia> AvaliacoesRecebidas { get; set; } = new List<AvaliacaoGuia>();
}
