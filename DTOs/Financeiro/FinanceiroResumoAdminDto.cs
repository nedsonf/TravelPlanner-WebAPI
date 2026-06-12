namespace TravelPlanner.Api.DTOs.Financeiro;

public class FinanceiroResumoAdminDto
{
    public decimal VolumeTotal { get; set; }
    public decimal TaxaPlataformaTotal { get; set; }
    public decimal RepasseGuiasTotal { get; set; }
    public decimal TaxaIntermediacaoPercentual { get; set; }
    public int QuantidadeVendas { get; set; }
}
