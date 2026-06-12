using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Transporte
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViagemId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Companhia { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string CodigoReserva { get; set; } = string.Empty;

    [Required]
    public DateTime DataPartida { get; set; }

    [ForeignKey(nameof(ViagemId))]
    public Viagem Viagem { get; set; } = null!;
}
