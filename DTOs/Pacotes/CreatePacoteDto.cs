using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.Pacotes;

public class CreatePacoteDto
{
    [Required]
    [MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Descricao { get; set; }

    [Required]
    public int DestinoId { get; set; }

    public int? HospedagemId { get; set; }

    [Required]
    public DateTime DataInicio { get; set; }

    [Required]
    public DateTime DataFim { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Preco { get; set; }

    public List<string> Dias { get; set; } = [];
}
