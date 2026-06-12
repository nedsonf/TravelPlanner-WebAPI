using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Hospedagens;
using TravelPlanner.Api.Models;

namespace TravelPlanner.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/hospedagens")]
public class HospedagemController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HospedagemController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HospedagemDto>>> Listar()
    {
        var hospedagens = await _context.Hospedagens
            .Include(h => h.Destino)
            .OrderBy(h => h.Nome)
            .Select(h => new HospedagemDto
            {
                Id = h.Id,
                Nome = h.Nome,
                Endereco = h.Endereco,
                DestinoId = h.DestinoId,
                DestinoNome = h.Destino.Cidade,
                CheckIn = h.CheckIn,
                CheckOut = h.CheckOut
            })
            .ToListAsync();

        return Ok(hospedagens);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<HospedagemDto>> Criar([FromBody] CreateHospedagemDto dto)
    {
        var destino = await _context.Destinos.FindAsync(dto.DestinoId);
        if (destino is null)
            return BadRequest(new { message = $"Destino {dto.DestinoId} não encontrado." });

        var hospedagem = new Hospedagem
        {
            DestinoId = dto.DestinoId,
            Nome = dto.Nome,
            Endereco = dto.Endereco,
            CheckIn = dto.CheckIn ?? DateTime.UtcNow.AddDays(30),
            CheckOut = dto.CheckOut ?? DateTime.UtcNow.AddDays(37)
        };

        _context.Hospedagens.Add(hospedagem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Listar), new { id = hospedagem.Id }, new HospedagemDto
        {
            Id = hospedagem.Id,
            Nome = hospedagem.Nome,
            Endereco = hospedagem.Endereco,
            DestinoId = hospedagem.DestinoId,
            DestinoNome = destino.Cidade,
            CheckIn = hospedagem.CheckIn,
            CheckOut = hospedagem.CheckOut
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remover(int id)
    {
        var hospedagem = await _context.Hospedagens.FindAsync(id);
        if (hospedagem is null)
            return NotFound(new { message = $"Hospedagem {id} não encontrada." });

        var possuiReservas = await _context.Reservas.AnyAsync(r => r.HospedagemId == id);
        if (possuiReservas)
            return Conflict(new { message = "Hospedagem possui reservas vinculadas e não pode ser removida." });

        _context.Hospedagens.Remove(hospedagem);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
