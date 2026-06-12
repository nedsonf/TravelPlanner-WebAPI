namespace TravelPlanner.Api.DTOs.Public;

public class HospedagemPublicoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public int DestinoId { get; set; }
    public string DestinoNome { get; set; } = string.Empty;
    public decimal ValorDiariaReferencia { get; set; }
}
