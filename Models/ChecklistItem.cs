using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class ChecklistItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViagemId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Item { get; set; } = string.Empty;

    public bool Concluido { get; set; }

    [ForeignKey(nameof(ViagemId))]
    public Viagem Viagem { get; set; } = null!;
}
