using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Viagem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViajanteId { get; set; }

    [Required]
    public int DestinoId { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    [Required]
    public DateTime DataFim { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Planejada";

    [ForeignKey(nameof(ViajanteId))]
    public Viajante Viajante { get; set; } = null!;

    [ForeignKey(nameof(DestinoId))]
    public Destino Destino { get; set; } = null!;

    public ICollection<RoteiroDiario> RoteirosDiarios { get; set; } = new List<RoteiroDiario>();
    public ICollection<Transporte> Transportes { get; set; } = new List<Transporte>();
    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
    public ICollection<ChecklistItem> ChecklistItens { get; set; } = new List<ChecklistItem>();
    public ICollection<Documento> Documentos { get; set; } = new List<Documento>();
}
