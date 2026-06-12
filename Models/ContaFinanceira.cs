using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class ContaFinanceira
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViajanteId { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal SaldoTotal { get; set; }

    [Required]
    [MaxLength(3)]
    public string MoedaPadrao { get; set; } = "BRL";

    [ForeignKey(nameof(ViajanteId))]
    public Viajante Viajante { get; set; } = null!;
}
