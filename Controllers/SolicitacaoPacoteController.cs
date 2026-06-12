using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Solicitacoes;
using TravelPlanner.Api.Models;
using TravelPlanner.Api.Services;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/solicitacoes-pacote")]
public class SolicitacaoPacoteController : ControllerBase
{
    private static readonly HashSet<string> CategoriasHotelValidas =
        new(StringComparer.OrdinalIgnoreCase) { "Entrada", "Padrao", "Conforto", "Luxo" };

    private readonly ApplicationDbContext _context;
    private readonly IPacoteService _pacoteService;

    public SolicitacaoPacoteController(ApplicationDbContext context, IPacoteService pacoteService)
    {
        _context = context;
        _pacoteService = pacoteService;
    }

    [HttpPost]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<SolicitacaoPacoteDto>> Criar([FromBody] CreateSolicitacaoPacoteDto dto)
    {
        if (!CategoriasHotelValidas.Contains(dto.CategoriaHotel))
            return BadRequest(new { message = "Categoria de hotel inválida. Use: Entrada, Padrao, Conforto ou Luxo." });

        if (dto.DataFim.Date < dto.DataInicio.Date)
            return BadRequest(new { message = "A data de volta deve ser igual ou posterior à data de ida." });

        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var guia = await _context.Guias.FindAsync(dto.GuiaId);
        if (guia is null)
            return BadRequest(new { message = "Guia não encontrado." });

        var destino = await _context.Destinos.FindAsync(dto.DestinoId);
        if (destino is null)
            return BadRequest(new { message = "Destino não encontrado." });

        var solicitacao = new SolicitacaoPacote
        {
            GuiaId = dto.GuiaId,
            ViajanteId = viajanteId.Value,
            DestinoId = dto.DestinoId,
            DataInicio = dto.DataInicio.Date,
            DataFim = dto.DataFim.Date,
            CategoriaHotel = NormalizarCategoria(dto.CategoriaHotel),
            Observacoes = dto.Observacoes,
            Status = "Pendente",
            CriadoEm = DateTime.UtcNow
        };

        _context.SolicitacoesPacote.Add(solicitacao);

        var guiaUsuarioId = await _context.Guias
            .Where(g => g.Id == dto.GuiaId)
            .Select(g => g.UsuarioId)
            .FirstAsync();

        _context.Notificacoes.Add(new Notificacao
        {
            UsuarioId = guiaUsuarioId,
            Mensagem = $"Novo pedido de pacote personalizado para {destino.Cidade}.",
            Lida = false,
            DataEnvio = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(await MapearDtoAsync(solicitacao.Id));
    }

    [HttpGet("minhas")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<IEnumerable<SolicitacaoPacoteDto>>> ObterMinhas()
    {
        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var ids = await _context.SolicitacoesPacote
            .Where(s => s.ViajanteId == viajanteId)
            .OrderByDescending(s => s.CriadoEm)
            .Select(s => s.Id)
            .ToListAsync();

        var lista = new List<SolicitacaoPacoteDto>();
        foreach (var id in ids)
            lista.Add(await MapearDtoAsync(id));

        return Ok(lista);
    }

    [HttpGet("pedidos")]
    [Authorize(Roles = "Guia")]
    public async Task<ActionResult<IEnumerable<SolicitacaoPacoteDto>>> ObterPedidos()
    {
        var guiaId = await ObterGuiaIdAsync();
        if (guiaId is null)
            return Forbid();

        var ids = await _context.SolicitacoesPacote
            .Where(s => s.GuiaId == guiaId)
            .OrderByDescending(s => s.CriadoEm)
            .Select(s => s.Id)
            .ToListAsync();

        var lista = new List<SolicitacaoPacoteDto>();
        foreach (var id in ids)
            lista.Add(await MapearDtoAsync(id));

        return Ok(lista);
    }

    [HttpPost("{id:int}/proposta")]
    [Authorize(Roles = "Guia")]
    public async Task<ActionResult<SolicitacaoPacoteDto>> EnviarProposta(int id, [FromBody] ResponderSolicitacaoDto dto)
    {
        var guiaId = await ObterGuiaIdAsync();
        if (guiaId is null)
            return Forbid();

        var solicitacao = await _context.SolicitacoesPacote
            .Include(s => s.Destino)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (solicitacao is null)
            return NotFound();

        if (solicitacao.GuiaId != guiaId)
            return Forbid();

        if (solicitacao.Status != "Pendente")
            return BadRequest(new { message = "Este pedido já foi respondido." });

        if (dto.Preco <= 0)
            return BadRequest(new { message = "Informe o preço do pacote." });

        Hospedagem? hospedagem = null;
        if (dto.HospedagemId is not null)
        {
            hospedagem = await _context.Hospedagens.FindAsync(dto.HospedagemId.Value);
            if (hospedagem is null || hospedagem.DestinoId != solicitacao.DestinoId)
                return BadRequest(new { message = "Hospedagem inválida para o destino do pedido." });
        }

        var pacote = new Pacote
        {
            GuiaId = guiaId,
            ViajanteId = solicitacao.ViajanteId,
            DestinoId = solicitacao.DestinoId,
            HospedagemId = dto.HospedagemId,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            DataInicio = solicitacao.DataInicio,
            DataFim = solicitacao.DataFim,
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

        solicitacao.PacoteId = pacote.Id;
        solicitacao.Status = "PropostaEnviada";
        solicitacao.RespondidoEm = DateTime.UtcNow;

        var viajanteUsuarioId = await _context.Viajantes
            .Where(v => v.Id == solicitacao.ViajanteId)
            .Select(v => v.UsuarioId)
            .FirstAsync();

        _context.Notificacoes.Add(new Notificacao
        {
            UsuarioId = viajanteUsuarioId,
            Mensagem = $"O guia enviou uma proposta para sua viagem a {solicitacao.Destino.Cidade}: {dto.Titulo} — {dto.Preco:C}.",
            Lida = false,
            DataEnvio = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(await MapearDtoAsync(id));
    }

    [HttpPost("{id:int}/confirmar")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<object>> Confirmar(int id)
    {
        var viajanteId = await ObterViajanteIdAsync();
        if (viajanteId is null)
            return Forbid();

        var usuarioId = ObterUsuarioId()!.Value;

        var solicitacao = await _context.SolicitacoesPacote
            .FirstOrDefaultAsync(s => s.Id == id);

        if (solicitacao is null)
            return NotFound();

        if (solicitacao.ViajanteId != viajanteId)
            return Forbid();

        if (solicitacao.Status != "PropostaEnviada" || solicitacao.PacoteId is null)
            return BadRequest(new { message = "Não há proposta disponível para confirmar." });

        var resultado = await _pacoteService.ReservarPacoteAsync(solicitacao.PacoteId.Value, usuarioId);
        if (resultado is null)
            return BadRequest(new { message = "Não foi possível confirmar a reserva." });

        solicitacao.Status = "Confirmada";
        await _context.SaveChangesAsync();

        return Ok(resultado);
    }

    private async Task<SolicitacaoPacoteDto> MapearDtoAsync(int id)
    {
        return await _context.SolicitacoesPacote
            .Where(s => s.Id == id)
            .Select(s => new SolicitacaoPacoteDto
            {
                Id = s.Id,
                GuiaId = s.GuiaId,
                GuiaNome = s.Guia.Usuario.Nome,
                DestinoId = s.DestinoId,
                DestinoNome = s.Destino.Cidade,
                DataInicio = s.DataInicio,
                DataFim = s.DataFim,
                CategoriaHotel = s.CategoriaHotel,
                Observacoes = s.Observacoes,
                Status = s.Status,
                PacoteId = s.PacoteId,
                PacoteTitulo = s.Pacote != null ? s.Pacote.Titulo : null,
                PacotePreco = s.Pacote != null ? s.Pacote.Preco : null,
                PacoteHospedagem = s.Pacote != null && s.Pacote.Hospedagem != null
                    ? s.Pacote.Hospedagem.Nome
                    : (s.Pacote != null ? "A combinar" : null),
                ViajanteNome = s.Viajante.Usuario.Nome,
                CriadoEm = s.CriadoEm
            })
            .FirstAsync();
    }

    private static string NormalizarCategoria(string categoria) =>
        categoria.ToLower() switch
        {
            "entrada" => "Entrada",
            "padrao" or "padrão" => "Padrao",
            "conforto" => "Conforto",
            "luxo" => "Luxo",
            _ => categoria
        };

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

    private async Task<int?> ObterGuiaIdAsync()
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId is null)
            return null;

        return await _context.Guias
            .Where(g => g.UsuarioId == usuarioId.Value)
            .Select(g => (int?)g.Id)
            .FirstOrDefaultAsync();
    }

    private int? ObterUsuarioId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : null;
    }
}
