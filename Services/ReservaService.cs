using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Reservas;
using TravelPlanner.Api.Models;

namespace TravelPlanner.Api.Services;

public class ReservaService : IReservaService
{
    private const string CategoriaHospedagem = "Hospedagem";

    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReservaService> _logger;

    public ReservaService(ApplicationDbContext context, ILogger<ReservaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ReservaCriadaDto?> CriarReservaComDespesaAsync(CreateReservaDto dto)
    {
        var viagemExiste = await _context.Viagens.AnyAsync(v => v.Id == dto.ViagemId);
        if (!viagemExiste)
        {
            _logger.LogWarning("Viagem {ViagemId} não encontrada ao criar reserva.", dto.ViagemId);
            return null;
        }

        var hospedagem = await _context.Hospedagens
            .FirstOrDefaultAsync(h => h.Id == dto.HospedagemId);

        if (hospedagem is null)
        {
            _logger.LogWarning("Hospedagem {HospedagemId} não encontrada ao criar reserva.", dto.HospedagemId);
            return null;
        }

        var categoria = await _context.CategoriasDespesa
            .FirstOrDefaultAsync(c => c.Nome == CategoriaHospedagem);

        if (categoria is null)
        {
            _logger.LogWarning("Categoria de despesa '{Categoria}' não encontrada.", CategoriaHospedagem);
            return null;
        }

        var reserva = new Reserva
        {
            ViagemId = dto.ViagemId,
            HospedagemId = dto.HospedagemId,
            Status = dto.Status,
            ValorTotal = dto.ValorTotal
        };

        var despesaDescricao = $"Reserva de hospedagem - {hospedagem.Nome}";

        var despesa = new Despesa
        {
            ViagemId = dto.ViagemId,
            CategoriaDespesaId = categoria.Id,
            Descricao = despesaDescricao,
            Valor = dto.ValorTotal,
            Data = DateTime.UtcNow,
            Reserva = reserva
        };

        _context.Reservas.Add(reserva);
        _context.Despesas.Add(despesa);

        await _context.SaveChangesAsync();

        return new ReservaCriadaDto
        {
            ReservaId = reserva.Id,
            DespesaId = despesa.Id,
            ViagemId = reserva.ViagemId,
            HospedagemId = reserva.HospedagemId,
            Status = reserva.Status,
            ValorTotal = reserva.ValorTotal,
            DespesaDescricao = despesaDescricao
        };
    }
}
