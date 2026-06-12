using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Pacote
{
    [Key]
    public int Id { get; set; }

    public int? GuiaId { get; set; }

    public int? ViajanteId { get; set; }

    [Required]
    public int DestinoId { get; set; }

    public int? HospedagemId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    [Required]
    public DateTime DataFim { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Preco { get; set; }

    [ForeignKey(nameof(GuiaId))]
    public Guia? Guia { get; set; }

    [ForeignKey(nameof(ViajanteId))]
    public Viajante? Viajante { get; set; }

    [ForeignKey(nameof(DestinoId))]
    public Destino Destino { get; set; } = null!;

    [ForeignKey(nameof(HospedagemId))]
    public Hospedagem? Hospedagem { get; set; }

    public ICollection<PacoteDia> Dias { get; set; } = new List<PacoteDia>();
}
