using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelPlanner.Api.Models;

public class Hospedagem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int DestinoId { get; set; }

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Endereco { get; set; } = string.Empty;

    [Required]
    public DateTime CheckIn { get; set; }

    [Required]
    public DateTime CheckOut { get; set; }

    [ForeignKey(nameof(DestinoId))]
    public Destino Destino { get; set; } = null!;

    public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
