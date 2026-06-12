using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class PacoteDia
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int PacoteId { get; set; }

    [Required]
    [Range(1, 365)]
    public int NumeroDia { get; set; }

    [Required]
    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [ForeignKey(nameof(PacoteId))]
    public Pacote Pacote { get; set; } = null!;
}
