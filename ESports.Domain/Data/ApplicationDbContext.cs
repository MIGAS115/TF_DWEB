using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Models;

namespace ESports.Domain.Data;

/// <summary>
/// Contexto principal da base de dados responsável pelo mapeamento das entidades e gestão do sistema de autenticação Identity.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<MyUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tabela representativa das equipas.
    /// </summary>
    public DbSet<Team> Teams { get; set; } = null!;

    /// <summary>
    /// Tabela representativa dos jogos agendados ou concluídos.
    /// </summary>
    public DbSet<ESports.Domain.Models.Match> Matches { get; set; } = null!;

    /// <summary>
    /// Tabela intermédia de associação N:M para registo das equipas favoritas de cada utilizador.
    /// </summary>
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Admin> Admins { get; set; } = null!;
    public DbSet<Normal> Normals { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<TournamentTeam> TournamentTeams { get; set; } = null!;

    /// <summary>
    /// Configuração das regras e restrições de relacionamento da base de dados através da Fluent API.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ESports.Domain.Models.Match>()
            .HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamFK)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ESports.Domain.Models.Match>()
            .HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamFK)
            .OnDelete(DeleteBehavior.Restrict);
    }
}