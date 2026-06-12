using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Notificacao
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Mensagem { get; set; } = string.Empty;

    public bool Lida { get; set; }

    [Required]
    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UsuarioId))]
    public Usuario Usuario { get; set; } = null!;
}
