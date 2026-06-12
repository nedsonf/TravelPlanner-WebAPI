namespace TravelPlanner.Api.DTOs.Solicitacoes;

public class SolicitacaoPacoteDto
{
    public int Id { get; set; }
    public int GuiaId { get; set; }
    public string? GuiaNome { get; set; }
    public int DestinoId { get; set; }
    public string DestinoNome { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string CategoriaHotel { get; set; } = string.Empty;
    public string? Observacoes { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? PacoteId { get; set; }
    public string? PacoteTitulo { get; set; }
    public decimal? PacotePreco { get; set; }
    public string? PacoteHospedagem { get; set; }
    public string? ViajanteNome { get; set; }
    public DateTime CriadoEm { get; set; }
}

public class CreateSolicitacaoPacoteDto
{
    public int GuiaId { get; set; }
    public int DestinoId { get; set; }
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string CategoriaHotel { get; set; } = "Padrao";
    public string? Observacoes { get; set; }
}

public class ResponderSolicitacaoDto
{
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int? HospedagemId { get; set; }
    public decimal Preco { get; set; }
    public List<string> Dias { get; set; } = [];
}
