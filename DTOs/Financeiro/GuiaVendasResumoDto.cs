namespace TravelPlanner.Api.DTOs.Financeiro;

public class GuiaVendasResumoDto
{
    public decimal FaturamentoBrutoTotal { get; set; }
    public decimal TaxaPlataformaTotal { get; set; }
    public decimal FaturamentoLiquidoTotal { get; set; }
    public decimal TaxaIntermediacaoPercentual { get; set; }
    public List<GuiaVendaDto> Vendas { get; set; } = [];
}
