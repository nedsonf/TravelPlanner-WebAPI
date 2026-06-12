using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Data;

namespace TravelPlanner.Api.Services;

public static class GuiaRatingService
{
    public static async Task AtualizarRatingAsync(ApplicationDbContext context, int guiaId)
    {
        var stats = await context.AvaliacoesGuias
            .Where(a => a.GuiaId == guiaId)
            .GroupBy(_ => 1)
            .Select(g => new { Media = g.Average(a => a.Nota), Total = g.Count() })
            .FirstOrDefaultAsync();

        var guia = await context.Guias.FindAsync(guiaId);
        if (guia is null)
            return;

        guia.Rating = stats is null ? 0 : Math.Round((decimal)stats.Media, 1);
        await context.SaveChangesAsync();
    }
}
