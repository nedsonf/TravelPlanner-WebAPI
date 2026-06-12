using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class RoteiroDiario
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViagemId { get; set; }

    [Required]
    public DateTime Data { get; set; }

    [Required]
    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [ForeignKey(nameof(ViagemId))]
    public Viagem Viagem { get; set; } = null!;

    public ICollection<Atividade> Atividades { get; set; } = new List<Atividade>();
}
