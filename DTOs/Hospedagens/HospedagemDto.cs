namespace TravelPlanner.Api.DTOs.Hospedagens;

public class HospedagemDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public int DestinoId { get; set; }
    public string DestinoNome { get; set; } = string.Empty;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}
