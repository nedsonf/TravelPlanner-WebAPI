using Microsoft.EntityFrameworkCore;
using TravelPlanner.Api.Models;

namespace TravelPlanner.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Viajante> Viajantes => Set<Viajante>();
    public DbSet<Guia> Guias => Set<Guia>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Destino> Destinos => Set<Destino>();
    public DbSet<Viagem> Viagens => Set<Viagem>();
    public DbSet<RoteiroDiario> RoteirosDiarios => Set<RoteiroDiario>();
    public DbSet<Atividade> Atividades => Set<Atividade>();
    public DbSet<Transporte> Transportes => Set<Transporte>();
    public DbSet<PontoTuristico> PontosTuristicos => Set<PontoTuristico>();
    public DbSet<Hospedagem> Hospedagens => Set<Hospedagem>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();
    public DbSet<ContaFinanceira> ContasFinanceiras => Set<ContaFinanceira>();
    public DbSet<CategoriaDespesa> CategoriasDespesa => Set<CategoriaDespesa>();
    public DbSet<Despesa> Despesas => Set<Despesa>();
    public DbSet<ChecklistItem> ChecklistItens => Set<ChecklistItem>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();
    public DbSet<Pacote> Pacotes => Set<Pacote>();
    public DbSet<PacoteDia> PacoteDias => Set<PacoteDia>();
    public DbSet<AvaliacaoGuia> AvaliacoesGuias => Set<AvaliacaoGuia>();
    public DbSet<SolicitacaoPacote> SolicitacoesPacote => Set<SolicitacaoPacote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- Relacionamentos 1:1 ---
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Viajante)
            .WithOne(v => v.Usuario)
            .HasForeignKey<Viajante>(v => v.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Guia)
            .WithOne(g => g.Usuario)
            .HasForeignKey<Guia>(g => g.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Administrador)
            .WithOne(a => a.Usuario)
            .HasForeignKey<Administrador>(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Viajante>()
            .HasOne(v => v.ContaFinanceira)
            .WithOne(c => c.Viajante)
            .HasForeignKey<ContaFinanceira>(c => c.ViajanteId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Identidade e papéis ---
        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.Usuario)
            .WithMany(u => u.Avaliacoes)
            .HasForeignKey(a => a.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notificacao>()
            .HasOne(n => n.Usuario)
            .WithMany(u => u.Notificacoes)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Destino e dependentes ---
        modelBuilder.Entity<Viagem>()
            .HasOne(v => v.Destino)
            .WithMany(d => d.Viagens)
            .HasForeignKey(v => v.DestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PontoTuristico>()
            .HasOne(p => p.Destino)
            .WithMany(d => d.PontosTuristicos)
            .HasForeignKey(p => p.DestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Hospedagem>()
            .HasOne(h => h.Destino)
            .WithMany(d => d.Hospedagens)
            .HasForeignKey(h => h.DestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Avaliacao>()
            .HasOne(a => a.PontoTuristico)
            .WithMany(p => p.Avaliacoes)
            .HasForeignKey(a => a.PontoTuristicoId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- Viagem e dependentes ---
        modelBuilder.Entity<Viagem>()
            .HasOne(v => v.Viajante)
            .WithMany(vj => vj.Viagens)
            .HasForeignKey(v => v.ViajanteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Filhos diretos da viagem (caminho único de exclusão)
        modelBuilder.Entity<RoteiroDiario>()
            .HasOne(r => r.Viagem)
            .WithMany(v => v.RoteirosDiarios)
            .HasForeignKey(r => r.ViagemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Atividade>()
            .HasOne(a => a.RoteiroDiario)
            .WithMany(r => r.Atividades)
            .HasForeignKey(a => a.RoteiroDiarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Transporte>()
            .HasOne(t => t.Viagem)
            .WithMany(v => v.Transportes)
            .HasForeignKey(t => t.ViagemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChecklistItem>()
            .HasOne(c => c.Viagem)
            .WithMany(v => v.ChecklistItens)
            .HasForeignKey(c => c.ViagemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Documento>()
            .HasOne(d => d.Viagem)
            .WithMany(v => v.Documentos)
            .HasForeignKey(d => d.ViagemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Reserva: evita caminhos múltiplos entre Viagem ↔ Hospedagem ↔ Despesa
        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Viagem)
            .WithMany(v => v.Reservas)
            .HasForeignKey(r => r.ViagemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Hospedagem)
            .WithMany(h => h.Reservas)
            .HasForeignKey(r => r.HospedagemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Guia)
            .WithMany()
            .HasForeignKey(r => r.GuiaId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Reserva>()
            .HasOne(r => r.Pacote)
            .WithMany()
            .HasForeignKey(r => r.PacoteId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Despesa: FKs para Viagem e Reserva não podem cascatear (caminhos múltiplos)
        modelBuilder.Entity<Despesa>()
            .HasOne(d => d.Viagem)
            .WithMany(v => v.Despesas)
            .HasForeignKey(d => d.ViagemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Despesa>()
            .HasOne(d => d.CategoriaDespesa)
            .WithMany(c => c.Despesas)
            .HasForeignKey(d => d.CategoriaDespesaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Despesa>()
            .HasOne(d => d.Reserva)
            .WithMany(r => r.Despesas)
            .HasForeignKey(d => d.ReservaId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // --- Pacotes (vitrine criada pelo Guia) ---
        modelBuilder.Entity<Pacote>()
            .HasOne(p => p.Guia)
            .WithMany()
            .HasForeignKey(p => p.GuiaId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Pacote>()
            .HasOne(p => p.Destino)
            .WithMany()
            .HasForeignKey(p => p.DestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Pacote>()
            .HasOne(p => p.Hospedagem)
            .WithMany()
            .HasForeignKey(p => p.HospedagemId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<PacoteDia>()
            .HasOne(d => d.Pacote)
            .WithMany(p => p.Dias)
            .HasForeignKey(d => d.PacoteId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Pacote>()
            .HasOne(p => p.Viajante)
            .WithMany()
            .HasForeignKey(p => p.ViajanteId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitacaoPacote>()
            .HasOne(s => s.Pacote)
            .WithMany()
            .HasForeignKey(s => s.PacoteId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AvaliacaoGuia>()
            .HasOne(a => a.Guia)
            .WithMany(g => g.AvaliacoesRecebidas)
            .HasForeignKey(a => a.GuiaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AvaliacaoGuia>()
            .HasOne(a => a.Viajante)
            .WithMany()
            .HasForeignKey(a => a.ViajanteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AvaliacaoGuia>()
            .HasOne(a => a.Reserva)
            .WithMany()
            .HasForeignKey(a => a.ReservaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AvaliacaoGuia>()
            .HasIndex(a => a.ReservaId)
            .IsUnique();

        modelBuilder.Entity<SolicitacaoPacote>()
            .HasOne(s => s.Guia)
            .WithMany()
            .HasForeignKey(s => s.GuiaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitacaoPacote>()
            .HasOne(s => s.Viajante)
            .WithMany()
            .HasForeignKey(s => s.ViajanteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SolicitacaoPacote>()
            .HasOne(s => s.Destino)
            .WithMany()
            .HasForeignKey(s => s.DestinoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
