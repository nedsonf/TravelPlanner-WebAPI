using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Reservas;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/reservas")]
public class ReservaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReservaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("viagem/{viagemId:int}")]
    [Authorize(Roles = "Viajante")]
    public async Task<ActionResult<IEnumerable<ReservaDto>>> ListarPorViagem(int viagemId)
    {
        var viagemExiste = await _context.Viagens.AnyAsync(v => v.Id == viagemId);
        if (!viagemExiste)
            return NotFound(new { message = $"Viagem {viagemId} não encontrada." });

        var reservas = await _context.Reservas
            .Include(r => r.Hospedagem)
            .Include(r => r.Pacote).ThenInclude(p => p!.Hospedagem)
            .Where(r => r.ViagemId == viagemId)
            .OrderByDescending(r => r.Id)
            .Select(r => new ReservaDto
            {
                Id = r.Id,
                Hospedagem = r.Hospedagem != null
                    ? r.Hospedagem.Nome
                    : (r.Pacote != null && r.Pacote.Hospedagem != null
                        ? r.Pacote.Hospedagem.Nome
                        : "A combinar"),
                PacoteTitulo = r.Pacote != null ? r.Pacote.Titulo : null,
                Status = r.Status,
                ValorTotal = r.ValorTotal,
                ViagemId = r.ViagemId
            })
            .ToListAsync();

        return Ok(reservas);
    }

    [HttpPost]
    [Authorize(Roles = "Viajante")]
    public ActionResult CriarReserva([FromBody] CreateReservaDto dto)
    {
        return BadRequest(new
        {
            message = "Reserva de hotel avulsa não é permitida. Escolha um pacote de um guia — o hotel vem na oferta (ou como \"A combinar\")."
        });
    }
}
