using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.Hospedagens;

public class CreateHospedagemDto
{
    [Required]
    public int DestinoId { get; set; } = 1;

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(250)]
    public string Endereco { get; set; } = string.Empty;

    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
}
