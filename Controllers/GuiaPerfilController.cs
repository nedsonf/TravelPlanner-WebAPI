using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Guias;
using TravelPlanner.Api.Models;
using TravelPlanner.Api.Services;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/guias")]
public class GuiaPerfilController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GuiaPerfilController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id:int}/perfil")]
    public async Task<ActionResult<GuiaPerfilDto>> ObterPerfil(int id)
    {
        var guia = await _context.Guias
            .Include(g => g.Usuario)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (guia is null)
            return NotFound(new { message = $"Guia {id} não encontrado." });

        var avaliacoes = await _context.AvaliacoesGuias
            .Where(a => a.GuiaId == id)
            .Include(a => a.Viajante).ThenInclude(v => v.Usuario)
            .OrderByDescending(a => a.CriadoEm)
            .Take(20)
            .Select(a => new AvaliacaoGuiaDto
            {
                Nota = a.Nota,
                Comentario = a.Comentario,
                ViajanteNome = a.Viajante.Usuario.Nome,
                CriadoEm = a.CriadoEm
            })
            .ToListAsync();

        var total = await _context.AvaliacoesGuias.CountAsync(a => a.GuiaId == id);

        return Ok(new GuiaPerfilDto
        {
            Id = guia.Id,
            Nome = guia.Usuario.Nome,
            Especialidade = guia.Especialidade,
            Rating = guia.Rating,
            TotalAvaliacoes = total,
            Avaliacoes = avaliacoes
        });
    }

    [HttpPost("{id:int}/avaliacoes")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<AvaliacaoGuiaDto>> Avaliar(int id, [FromBody] CreateAvaliacaoGuiaDto dto)
    {
        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var reserva = await _context.Reservas
            .Include(r => r.Viagem)
            .FirstOrDefaultAsync(r => r.Id == dto.ReservaId);

        if (reserva is null)
            return NotFound(new { message = "Reserva não encontrada." });

        if (reserva.Viagem.ViajanteId != viajanteId)
            return Forbid();

        if (reserva.GuiaId != id)
            return BadRequest(new { message = "Esta reserva não pertence ao guia informado." });

        if (reserva.Status != "Pago" && reserva.Status != "Confirmada")
            return BadRequest(new { message = "Só é possível avaliar após fechar o pacote com o guia." });

        if (await _context.AvaliacoesGuias.AnyAsync(a => a.ReservaId == dto.ReservaId))
            return Conflict(new { message = "Você já avaliou esta viagem." });

        var viajante = await _context.Viajantes
            .Include(v => v.Usuario)
            .FirstAsync(v => v.Id == viajanteId.Value);

        var avaliacao = new AvaliacaoGuia
        {
            GuiaId = id,
            ViajanteId = viajanteId.Value,
            ReservaId = dto.ReservaId,
            Nota = dto.Nota,
            Comentario = dto.Comentario,
            CriadoEm = DateTime.UtcNow
        };

        _context.AvaliacoesGuias.Add(avaliacao);
        await _context.SaveChangesAsync();
        await GuiaRatingService.AtualizarRatingAsync(_context, id);

        return Ok(new AvaliacaoGuiaDto
        {
            Nota = avaliacao.Nota,
            Comentario = avaliacao.Comentario,
            ViajanteNome = viajante.Usuario.Nome,
            CriadoEm = avaliacao.CriadoEm
        });
    }

    [HttpGet("minhas-reservas-avaliaveis")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<IEnumerable<object>>> ListarReservasAvaliaveis()
    {
        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var reservas = await _context.Reservas
            .Include(r => r.Guia).ThenInclude(g => g!.Usuario)
            .Include(r => r.Pacote)
            .Where(r => r.Viagem.ViajanteId == viajanteId
                && r.GuiaId != null
                && (r.Status == "Pago" || r.Status == "Confirmada")
                && r.PacoteId != null)
            .Where(r => !_context.AvaliacoesGuias.Any(a => a.ReservaId == r.Id))
            .Select(r => new
            {
                r.Id,
                GuiaId = r.GuiaId,
                GuiaNome = r.Guia != null ? r.Guia.Usuario.Nome : null,
                PacoteTitulo = r.Pacote != null ? r.Pacote.Titulo : null,
                r.ValorTotal
            })
            .ToListAsync();

        return Ok(reservas);
    }

    private async Task<int?> ObterViajanteIdAsync()
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
            return null;

        return await _context.Viajantes
            .Where(v => v.UsuarioId == usuarioId.Value)
            .Select(v => (int?)v.Id)
            .FirstOrDefaultAsync();
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }
}
