using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Reserva
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViagemId { get; set; }

    public int? HospedagemId { get; set; }

    public int? GuiaId { get; set; }

    public int? PacoteId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Pendente";

    [Required]
    [Range(0, double.MaxValue)]
    public decimal ValorTotal { get; set; }

    [Range(0, double.MaxValue)]
    public decimal TaxaPlataforma { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ValorLiquidoGuia { get; set; }

    [ForeignKey(nameof(ViagemId))]
    public Viagem Viagem { get; set; } = null!;

    [ForeignKey(nameof(HospedagemId))]
    public Hospedagem? Hospedagem { get; set; }

    [ForeignKey(nameof(GuiaId))]
    public Guia? Guia { get; set; }

    [ForeignKey(nameof(PacoteId))]
    public Pacote? Pacote { get; set; }

    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}
