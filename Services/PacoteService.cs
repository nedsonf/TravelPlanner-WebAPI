using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TravelPlanner.Api.Configuration;
using TravelPlanner.Api.Data;
using TravelPlanner.Api.DTOs.Pacotes;
using TravelPlanner.Api.Models;

namespace TravelPlanner.Api.Services;

public class PacoteService : IPacoteService
{
    private const string CategoriaHospedagem = "Hospedagem";

    private readonly ApplicationDbContext _context;
    private readonly PlataformaSettings _plataforma;
    private readonly ILogger<PacoteService> _logger;

    public PacoteService(
        ApplicationDbContext context,
        IOptions<PlataformaSettings> plataforma,
        ILogger<PacoteService> logger)
    {
        _context = context;
        _plataforma = plataforma.Value;
        _logger = logger;
    }

    public async Task<PacoteReservadoDto?> ReservarPacoteAsync(int pacoteId, int usuarioId)
    {
        var viajante = await _context.Viajantes
            .FirstOrDefaultAsync(v => v.UsuarioId == usuarioId);

        if (viajante is null)
        {
            _logger.LogWarning("Usuário {UsuarioId} não possui perfil de viajante.", usuarioId);
            return null;
        }

        var pacote = await _context.Pacotes
            .Include(p => p.Destino)
            .Include(p => p.Hospedagem)
            .Include(p => p.Dias)
            .FirstOrDefaultAsync(p => p.Id == pacoteId);

        if (pacote is null)
        {
            _logger.LogWarning("Pacote {PacoteId} não encontrado.", pacoteId);
            return null;
        }

        if (pacote.ViajanteId.HasValue && pacote.ViajanteId != viajante.Id)
        {
            _logger.LogWarning("Pacote {PacoteId} é exclusivo para outro viajante.", pacoteId);
            return null;
        }

        var viagem = new Viagem
        {
            ViajanteId = viajante.Id,
            DestinoId = pacote.DestinoId,
            DataInicio = pacote.DataInicio,
            DataFim = pacote.DataFim,
            Status = "Planejando"
        };

        foreach (var dia in pacote.Dias.OrderBy(d => d.NumeroDia))
        {
            viagem.RoteirosDiarios.Add(new RoteiroDiario
            {
                Data = pacote.DataInicio.AddDays(dia.NumeroDia - 1),
                Descricao = dia.Descricao
            });
        }

        _context.Viagens.Add(viagem);

        Reserva? reserva = null;
        Despesa? despesa = null;

        if (pacote.Preco > 0)
        {
            var categoria = await _context.CategoriasDespesa
                .FirstOrDefaultAsync(c => c.Nome == CategoriaHospedagem);

            if (categoria is null)
            {
                _logger.LogWarning("Categoria '{Categoria}' não encontrada ao reservar pacote.", CategoriaHospedagem);
                return null;
            }

            var taxa = Math.Round(pacote.Preco * _plataforma.TaxaIntermediacaoPercentual / 100m, 2);
            var liquido = pacote.Preco - taxa;

            reserva = new Reserva
            {
                Viagem = viagem,
                HospedagemId = pacote.HospedagemId,
                GuiaId = pacote.GuiaId,
                PacoteId = pacote.Id,
                Status = "Pago",
                ValorTotal = pacote.Preco,
                TaxaPlataforma = taxa,
                ValorLiquidoGuia = liquido
            };

            despesa = new Despesa
            {
                Viagem = viagem,
                CategoriaDespesaId = categoria.Id,
                Descricao = $"Pacote: {pacote.Titulo}",
                Valor = pacote.Preco,
                Data = DateTime.UtcNow,
                Reserva = reserva
            };

            viagem.Status = "Pago";

            _context.Reservas.Add(reserva);
            _context.Despesas.Add(despesa);
        }

        await _context.SaveChangesAsync();

        return new PacoteReservadoDto
        {
            PacoteId = pacote.Id,
            ViagemId = viagem.Id,
            ReservaId = reserva?.Id,
            DespesaId = despesa?.Id,
            DestinoNome = pacote.Destino.Cidade,
            ValorTotal = pacote.Preco,
            DiasRoteiro = pacote.Dias.Count
        };
    }
}
