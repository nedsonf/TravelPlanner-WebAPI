namespace TravelPlanner.Api.DTOs.Pacotes;

public class PacoteDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int DestinoId { get; set; }
    public string DestinoNome { get; set; } = string.Empty;
    public string DestinoPais { get; set; } = string.Empty;
    public int? HospedagemId { get; set; }
    public string? HospedagemNome { get; set; }
    public int? GuiaId { get; set; }
    public string? GuiaNome { get; set; }
    public decimal? GuiaRating { get; set; }
    public int GuiaTotalAvaliacoes { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public decimal Preco { get; set; }
    public List<PacoteDiaDto> Dias { get; set; } = [];
}

public class PacoteDiaDto
{
    public int NumeroDia { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
