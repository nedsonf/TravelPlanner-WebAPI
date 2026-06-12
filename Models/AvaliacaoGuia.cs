using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class AvaliacaoGuia
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int GuiaId { get; set; }

    [Required]
    public int ViajanteId { get; set; }

    [Required]
    public int ReservaId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Nota { get; set; }

    [MaxLength(1000)]
    public string? Comentario { get; set; }

    [Required]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(GuiaId))]
    public Guia Guia { get; set; } = null!;

    [ForeignKey(nameof(ViajanteId))]
    public Viajante Viajante { get; set; } = null!;

    [ForeignKey(nameof(ReservaId))]
    public Reserva Reserva { get; set; } = null!;
}
