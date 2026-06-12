using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class SolicitacaoPacote
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int GuiaId { get; set; }

    [Required]
    public int ViajanteId { get; set; }

    [Required]
    public int DestinoId { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    [Required]
    public DateTime DataFim { get; set; }

    [Required]
    [MaxLength(30)]
    public string CategoriaHotel { get; set; } = "Padrao";

    [MaxLength(2000)]
    public string? Observacoes { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "Pendente";

    public int? PacoteId { get; set; }

    [Required]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public DateTime? RespondidoEm { get; set; }

    [ForeignKey(nameof(GuiaId))]
    public Guia Guia { get; set; } = null!;

    [ForeignKey(nameof(ViajanteId))]
    public Viajante Viajante { get; set; } = null!;

    [ForeignKey(nameof(DestinoId))]
    public Destino Destino { get; set; } = null!;

    [ForeignKey(nameof(PacoteId))]
    public Pacote? Pacote { get; set; }
}
