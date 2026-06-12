using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Documento
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ViagemId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Url]
    public string UrlArquivo { get; set; } = string.Empty;

    [ForeignKey(nameof(ViagemId))]
    public Viagem Viagem { get; set; } = null!;
}
