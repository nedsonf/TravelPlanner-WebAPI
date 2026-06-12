using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelPlanner.Api.Configuration;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Financeiro;

namespace TravelPlanner.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/financeiro")]
public class FinanceiroController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PlataformaSettings _plataforma;

    public FinanceiroController(ApplicationDbContext context, IOptions<PlataformaSettings> plataforma)
    {
        _context = context;
        _plataforma = plataforma.Value;
    }

    [HttpGet("resumo")]
    public async Task<ActionResult<FinanceiroResumoAdminDto>> ObterResumo()
    {
        var vendas = await _context.Reservas
            .Where(r => r.PacoteId != null)
            .ToListAsync();

        var volume = vendas.Sum(r => r.ValorTotal);
        var taxa = vendas.Sum(r => r.TaxaPlataforma);

        return Ok(new FinanceiroResumoAdminDto
        {
            VolumeTotal = volume,
            TaxaPlataformaTotal = taxa,
            RepasseGuiasTotal = vendas.Sum(r => r.ValorLiquidoGuia),
            TaxaIntermediacaoPercentual = _plataforma.TaxaIntermediacaoPercentual,
            QuantidadeVendas = vendas.Count
        });
    }

    [HttpGet("despesas")]
    public async Task<ActionResult<IEnumerable<DespesaExtratoDto>>> ListarDespesas()
    {
        var despesas = await _context.Despesas
            .Include(d => d.CategoriaDespesa)
            .OrderByDescending(d => d.Data)
            .Select(d => new DespesaExtratoDto
            {
                Id = d.Id,
                Descricao = d.Descricao,
                Categoria = d.CategoriaDespesa.Nome,
                Valor = d.Valor,
                Data = d.Data,
                ViagemId = d.ViagemId,
                ReservaId = d.ReservaId
            })
            .ToListAsync();

        return Ok(despesas);
    }

    [HttpGet("reservas")]
    public async Task<ActionResult<IEnumerable<ReservaExtratoDto>>> ListarReservas()
    {
        var reservas = await _context.Reservas
            .Include(r => r.Hospedagem)
            .Include(r => r.Pacote)
            .Include(r => r.Guia).ThenInclude(g => g!.Usuario)
            .OrderByDescending(r => r.Id)
            .Select(r => new ReservaExtratoDto
            {
                Id = r.Id,
                Hospedagem = r.Hospedagem != null
                    ? r.Hospedagem.Nome
                    : (r.Pacote != null ? $"Pacote: {r.Pacote.Titulo}" : "A combinar"),
                Status = r.Status,
                ValorTotal = r.ValorTotal,
                TaxaPlataforma = r.TaxaPlataforma,
                ValorLiquidoGuia = r.ValorLiquidoGuia,
                GuiaNome = r.Guia != null ? r.Guia.Usuario.Nome : null,
                ViagemId = r.ViagemId
            })
            .ToListAsync();

        return Ok(reservas);
    }

    [HttpDelete("despesas/{id}")]
    public async Task<IActionResult> RemoverDespesa(int id)
    {
        var despesa = await _context.Despesas.FindAsync(id);
        if (despesa is null)
            return NotFound(new { message = $"Despesa {id} não encontrada." });

        _context.Despesas.Remove(despesa);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
