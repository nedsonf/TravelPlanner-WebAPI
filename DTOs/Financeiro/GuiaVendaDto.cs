namespace TravelPlanner.Api.DTOs.Financeiro;

public class GuiaVendaDto
{
    public int ReservaId { get; set; }
    public string PacoteTitulo { get; set; } = string.Empty;
    public string Destino { get; set; } = string.Empty;
    public DateTime DataVenda { get; set; }
    public decimal FaturamentoBruto { get; set; }
    public decimal TaxaPlataforma { get; set; }
    public decimal FaturamentoLiquido { get; set; }
}
