using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Atividade
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RoteiroDiarioId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public TimeSpan HorarioInicio { get; set; }

    [Required]
    public TimeSpan HorarioFim { get; set; }

    [ForeignKey(nameof(RoteiroDiarioId))]
    public RoteiroDiario RoteiroDiario { get; set; } = null!;
}
