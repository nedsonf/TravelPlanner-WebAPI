using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelPlanner.Api.Configuration;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Financeiro;

namespace TravelPlanner.Api.Controllers;

[Authorize(Roles = "Guia")]
[ApiController]
[Route("api/financeiro/guia")]
public class GuiaFinanceiroController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly PlataformaSettings _plataforma;

    public GuiaFinanceiroController(ApplicationDbContext context, IOptions<PlataformaSettings> plataforma)
    {
        _context = context;
        _plataforma = plataforma.Value;
    }

    [HttpGet("vendas")]
    public async Task<ActionResult<GuiaVendasResumoDto>> ObterMinhasVendas()
    {
        var guiaId = await ObterGuiaIdAsync();
        if (guiaId is null)
            return Forbid();

        var vendas = await _context.Reservas
            .Include(r => r.Pacote)
            .Include(r => r.Viagem).ThenInclude(v => v.Destino)
            .Where(r => r.GuiaId == guiaId && r.PacoteId != null)
            .OrderByDescending(r => r.Id)
            .Select(r => new GuiaVendaDto
            {
                ReservaId = r.Id,
                PacoteTitulo = r.Pacote != null ? r.Pacote.Titulo : "Pacote",
                Destino = r.Viagem.Destino.Cidade,
                DataVenda = r.Viagem.DataInicio,
                FaturamentoBruto = r.ValorTotal,
                TaxaPlataforma = r.TaxaPlataforma,
                FaturamentoLiquido = r.ValorLiquidoGuia
            })
            .ToListAsync();

        return Ok(new GuiaVendasResumoDto
        {
            FaturamentoBrutoTotal = vendas.Sum(v => v.FaturamentoBruto),
            TaxaPlataformaTotal = vendas.Sum(v => v.TaxaPlataforma),
            FaturamentoLiquidoTotal = vendas.Sum(v => v.FaturamentoLiquido),
            TaxaIntermediacaoPercentual = _plataforma.TaxaIntermediacaoPercentual,
            Vendas = vendas
        });
    }

    private async Task<int?> ObterGuiaIdAsync()
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
            return null;

        var guia = await _context.Guias
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.UsuarioId == usuarioId.Value);

        return guia?.Id;
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }
}
