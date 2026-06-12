using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Avaliacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public int PontoTuristicoId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Nota { get; set; }

    [MaxLength(1000)]
    public string? Comentario { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;

    [ForeignKey(nameof(PontoTuristicoId))]
    public PontoTuristico PontoTuristico { get; set; } = null!;
}
