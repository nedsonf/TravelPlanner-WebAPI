namespace TravelPlanner.Api.DTOs.Guias;

public class GuiaPerfilDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int TotalAvaliacoes { get; set; }
    public List<AvaliacaoGuiaDto> Avaliacoes { get; set; } = [];
}

public class AvaliacaoGuiaDto
{
    public int Nota { get; set; }
    public string? Comentario { get; set; }
    public string ViajanteNome { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
}
