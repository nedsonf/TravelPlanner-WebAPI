using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Despesa
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViagemId { get; set; }

    [Required]
    public int CategoriaDespesaId { get; set; }

    [Required]
    [MaxLength(250)]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Valor { get; set; }

    [Required]
    public DateTime Data { get; set; }

    public int? ReservaId { get; set; }

    [ForeignKey(nameof(ViagemId))]
    public Viagem Viagem { get; set; } = null!;

    [ForeignKey(nameof(CategoriaDespesaId))]
    public CategoriaDespesa CategoriaDespesa { get; set; } = null!;

    [ForeignKey(nameof(ReservaId))]
    public Reserva? Reserva { get; set; }
}
