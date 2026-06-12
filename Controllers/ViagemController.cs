using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Viagens;
using TravelPlanner.Api.Models;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/viagens")]
public class ViagemController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ViagemController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("minha/carteira")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<IEnumerable<CarteiraItemDto>>> ObterCarteira()
    {
        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var itens = await _context.Viagens
            .Where(v => v.ViajanteId == viajanteId.Value && v.Reservas.Any())
            .OrderByDescending(v => v.DataInicio)
            .Select(v => new CarteiraItemDto
            {
                ViagemId = v.Id,
                Destino = v.Destino.Cidade,
                DataInicio = v.DataInicio,
                DataFim = v.DataFim,
                Status = v.Status,
                ValorTotal = v.Reservas.Sum(r => r.ValorTotal),
                PacoteTitulo = v.Reservas
                    .Where(r => r.Pacote != null)
                    .Select(r => r.Pacote!.Titulo)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(itens);
    }

    [HttpGet("minha/atual")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<ViagemDto>> ObterViagemAtual()
    {
        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var viagem = await BuscarViagemQuery(viajanteId.Value)
            .OrderByDescending(v => v.DataInicio)
            .FirstOrDefaultAsync();

        if (viagem is null)
            return NotFound(new { message = "Nenhuma viagem planejada ainda." });

        return Ok(viagem);
    }

    [HttpPost]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<ViagemDto>> Criar([FromBody] CreateViagemDto dto)
    {
        var validationError = ValidarDatas(dto.DataInicio, dto.DataFim);
        if (validationError is not null)
            return BadRequest(new { message = validationError });

        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var destino = await _context.Destinos.FindAsync(dto.DestinoId);
        if (destino is null)
            return BadRequest(new { message = $"Destino {dto.DestinoId} não encontrado." });

        var viagem = new Viagem
        {
            ViajanteId = viajanteId.Value,
            DestinoId = dto.DestinoId,
            DataInicio = dto.DataInicio.Date,
            DataFim = dto.DataFim.Date,
            Status = dto.Status
        };

        _context.Viagens.Add(viagem);
        await _context.SaveChangesAsync();
        await RemoverPlanejamentosOrfaosAsync(viajanteId.Value, viagem.Id);

        return CreatedAtAction(nameof(ObterViagemAtual), new ViagemDto
        {
            Id = viagem.Id,
            DestinoId = destino.Id,
            DestinoNome = destino.Cidade,
            DestinoPais = destino.Pais,
            DataInicio = viagem.DataInicio,
            DataFim = viagem.DataFim,
            Status = viagem.Status
        });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<ViagemDto>> Atualizar(int id, [FromBody] UpdateViagemDto dto)
    {
        var validationError = ValidarDatas(dto.DataInicio, dto.DataFim);
        if (validationError is not null)
            return BadRequest(new { message = validationError });

        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var viagem = await _context.Viagens
            .Include(v => v.Destino)
            .FirstOrDefaultAsync(v => v.Id == id && v.ViajanteId == viajanteId.Value);

        if (viagem is null)
            return NotFound(new { message = $"Viagem {id} não encontrada." });

        var destino = await _context.Destinos.FindAsync(dto.DestinoId);
        if (destino is null)
            return BadRequest(new { message = $"Destino {dto.DestinoId} não encontrado." });

        viagem.DestinoId = dto.DestinoId;
        viagem.DataInicio = dto.DataInicio.Date;
        viagem.DataFim = dto.DataFim.Date;
        viagem.Status = dto.Status;
        viagem.Destino = destino;

        await _context.SaveChangesAsync();
        await RemoverPlanejamentosOrfaosAsync(viajanteId.Value, viagem.Id);

        return Ok(new ViagemDto
        {
            Id = viagem.Id,
            DestinoId = destino.Id,
            DestinoNome = destino.Cidade,
            DestinoPais = destino.Pais,
            DataInicio = viagem.DataInicio,
            DataFim = viagem.DataFim,
            Status = viagem.Status
        });
    }

    private async Task RemoverPlanejamentosOrfaosAsync(int viajanteId, int viagemAtivaId)
    {
        var orfas = await _context.Viagens
            .Where(v => v.ViajanteId == viajanteId
                && v.Id != viagemAtivaId
                && v.Status == "Planejada"
                && !v.Reservas.Any())
            .ToListAsync();

        if (orfas.Count == 0)
            return;

        _context.Viagens.RemoveRange(orfas);
        await _context.SaveChangesAsync();
    }

    private async Task<int?> ObterViajanteIdAsync()
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
            return null;

        var viajante = await _context.Viajantes
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId.Value);

        return viajante?.Id;
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }

    private IQueryable<ViagemDto> BuscarViagemQuery(int viajanteId) =>
        _context.Viagens
            .Where(v => v.ViajanteId == viajanteId)
            .Select(v => new ViagemDto
            {
                Id = v.Id,
                DestinoId = v.DestinoId,
                DestinoNome = v.Destino.Cidade,
                DestinoPais = v.Destino.Pais,
                DataInicio = v.DataInicio,
                DataFim = v.DataFim,
                Status = v.Status
            });

    private static string? ValidarDatas(DateTime inicio, DateTime fim) =>
        fim.Date < inicio.Date ? "A data de término deve ser igual ou posterior à data de início." : null;
}
