using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Pacotes;
using TravelPlanner.Api.Models;
using TravelPlanner.Api.Services;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/pacotes")]
public class PacoteController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPacoteService _pacoteService;

    public PacoteController(ApplicationDbContext context, IPacoteService pacoteService)
    {
        _context = context;
        _pacoteService = pacoteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PacoteDto>>> Listar(
        [FromQuery] int? destinoId,
        [FromQuery] int? ano,
        [FromQuery] int? mes)
    {
        var query = _context.Pacotes
            .Where(p => p.ViajanteId == null)
            .AsQueryable();

        if (destinoId is not null)
            query = query.Where(p => p.DestinoId == destinoId);

        if (ano is not null && mes is not null)
            query = query.Where(p => p.DataInicio.Year == ano && p.DataInicio.Month == mes);

        var pacotes = await query
            .OrderBy(p => p.DataInicio)
            .Select(p => new PacoteDto
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Descricao = p.Descricao,
                DestinoId = p.DestinoId,
                DestinoNome = p.Destino.Cidade,
                DestinoPais = p.Destino.Pais,
                HospedagemId = p.HospedagemId,
                HospedagemNome = p.Hospedagem != null ? p.Hospedagem.Nome : null,
                GuiaId = p.GuiaId,
                GuiaNome = p.Guia != null ? p.Guia.Usuario.Nome : null,
                GuiaRating = p.Guia != null ? p.Guia.Rating : null,
                GuiaTotalAvaliacoes = p.Guia != null ? p.Guia.AvaliacoesRecebidas.Count : 0,
                DataInicio = p.DataInicio,
                DataFim = p.DataFim,
                Preco = p.Preco,
                Dias = p.Dias
                    .OrderBy(d => d.NumeroDia)
                    .Select(d => new PacoteDiaDto { NumeroDia = d.NumeroDia, Descricao = d.Descricao })
                    .ToList()
            })
            .ToListAsync();

        return Ok(pacotes);
    }

    [HttpGet("meus")]
    [Authorize(Roles = "Guia")]
    public async Task<ActionResult<IEnumerable<PacoteDto>>> ListarMeus()
    {
        var guiaId = await ObterGuiaIdAsync();
        if (guiaId is null)
            return Forbid();

        var pacotes = await _context.Pacotes
            .Where(p => p.GuiaId == guiaId)
            .Include(p => p.Destino)
            .Include(p => p.Hospedagem)
            .Include(p => p.Guia).ThenInclude(g => g!.Usuario)
            .Include(p => p.Dias)
            .OrderBy(p => p.DataInicio)
            .Select(p => new PacoteDto
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Descricao = p.Descricao,
                DestinoId = p.DestinoId,
                DestinoNome = p.Destino.Cidade,
                DestinoPais = p.Destino.Pais,
                HospedagemId = p.HospedagemId,
                HospedagemNome = p.Hospedagem != null ? p.Hospedagem.Nome : null,
                GuiaNome = p.Guia != null ? p.Guia.Usuario.Nome : null,
                DataInicio = p.DataInicio,
                DataFim = p.DataFim,
                Preco = p.Preco,
                Dias = p.Dias
                    .OrderBy(d => d.NumeroDia)
                    .Select(d => new PacoteDiaDto { NumeroDia = d.NumeroDia, Descricao = d.Descricao })
                    .ToList()
            })
            .ToListAsync();

        return Ok(pacotes);
    }

    [HttpPost]
    [Authorize(Roles = "Guia")]
    public async Task<ActionResult<PacoteDto>> Criar([FromBody] CreatePacoteDto dto)
    {
        if (dto.DataFim.Date < dto.DataInicio.Date)
            return BadRequest(new { message = "A data de término deve ser igual ou posterior à data de início." });

        var destino = await _context.Destinos.FindAsync(dto.DestinoId);
        if (destino is null)
            return BadRequest(new { message = $"Destino {dto.DestinoId} não encontrado." });

        Hospedagem? hospedagem = null;
        if (dto.HospedagemId is not null)
        {
            hospedagem = await _context.Hospedagens.FindAsync(dto.HospedagemId.Value);
            if (hospedagem is null)
                return BadRequest(new { message = $"Hospedagem {dto.HospedagemId} não encontrada." });

            if (hospedagem.DestinoId != dto.DestinoId)
                return BadRequest(new { message = "A hospedagem escolhida não pertence ao destino do pacote." });
        }

        var guiaId = await ObterGuiaIdAsync();
        if (guiaId is null)
            return BadRequest(new { message = "Perfil de guia não encontrado para publicar pacotes." });

        var pacote = new Pacote
        {
            GuiaId = guiaId,
            DestinoId = dto.DestinoId,
            HospedagemId = dto.HospedagemId,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            DataInicio = dto.DataInicio.Date,
            DataFim = dto.DataFim.Date,
            Preco = dto.Preco
        };

        var numeroDia = 1;
        foreach (var descricao in dto.Dias.Where(d => !string.IsNullOrWhiteSpace(d)))
        {
            pacote.Dias.Add(new PacoteDia { NumeroDia = numeroDia, Descricao = descricao.Trim() });
            numeroDia++;
        }

        _context.Pacotes.Add(pacote);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Listar), new { id = pacote.Id }, new PacoteDto
        {
            Id = pacote.Id,
            Titulo = pacote.Titulo,
            Descricao = pacote.Descricao,
            DestinoId = destino.Id,
            DestinoNome = destino.Cidade,
            DestinoPais = destino.Pais,
            HospedagemId = hospedagem?.Id,
            HospedagemNome = hospedagem?.Nome,
            DataInicio = pacote.DataInicio,
            DataFim = pacote.DataFim,
            Preco = pacote.Preco,
            Dias = pacote.Dias
                .OrderBy(d => d.NumeroDia)
                .Select(d => new PacoteDiaDto { NumeroDia = d.NumeroDia, Descricao = d.Descricao })
                .ToList()
        });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Guia")]
    public async Task<IActionResult> Remover(int id)
    {
        var guiaId = await ObterGuiaIdAsync();
        if (guiaId is null)
            return Forbid();

        var pacote = await _context.Pacotes.FindAsync(id);
        if (pacote is null)
            return NotFound(new { message = $"Pacote {id} não encontrado." });

        if (pacote.GuiaId != guiaId)
            return Forbid();

        _context.Pacotes.Remove(pacote);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}/reservar")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<PacoteReservadoDto>> Reservar(int id)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
            return Forbid();

        var resultado = await _pacoteService.ReservarPacoteAsync(id, usuarioId.Value);

        if (resultado is null)
            return BadRequest(new { message = "Não foi possível reservar o pacote. Verifique se o pacote existe e se seu perfil de viajante está completo." });

        return Ok(resultado);
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
