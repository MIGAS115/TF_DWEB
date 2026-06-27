using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Models;

namespace ESports.Domain.Data;

/// <summary>
/// Contexto principal da base de dados responsável pelo mapeamento das entidades e gestão do sistema de autenticação Identity.
/// Implementa uma arquitetura desacoplada onde o Identity gerencia estritamente a segurança física (IdentityUser)
/// e as tabelas de domínio gerem o perfil de negócio dos utilizadores.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    /// <summary>
    /// Construtor do contexto da base de dados que encaminha as opções de configuração para a classe base.
    /// </summary>
    /// <param name="options">Opções de configuração do DbContext.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Tabela base representativa de todos os utilizadores no contexto de negócio da plataforma.
    /// </summary>
    public DbSet<MyUser> AppUsers { get; set; } = null!;

    /// <summary>
    /// Filtro de tabela focado nos utilizadores com privilégios de Administração.
    /// </summary>
    public DbSet<Admin> Admins { get; set; } = null!;

    /// <summary>
    /// Filtro de tabela focado nos utilizadores regulares/normais da plataforma.
    /// </summary>
    public DbSet<RegularUser> RegularUsers { get; set; } = null!;

    /// <summary>
    /// Tabela representativa das equipas de e-sports.
    /// </summary>
    public DbSet<Team> Teams { get; set; } = null!;

    /// <summary>
    /// Tabela representativa dos jogos (Matches) agendados ou concluídos.
    /// </summary>
    public DbSet<Match> Matches { get; set; } = null!;

    /// <summary>
    /// Tabela intermédia de associação N:M para registo das equipas favoritas de cada utilizador.
    /// </summary>
    public DbSet<Favorite> Favorites { get; set; } = null!;

    /// <summary>
    /// Tabela representativa dos torneios e competições.
    /// </summary>
    public DbSet<Tournament> Tournaments { get; set; } = null!;

    /// <summary>
    /// Tabela intermédia de associação N:M para mapeamento das equipas inscritas em cada torneio.
    /// </summary>
    public DbSet<TournamentTeam> TournamentTeams { get; set; } = null!;

    /// <summary>
    /// Tabela representativa das categorias de jogos (CS2, LOL, DOTA2).
    /// </summary>
    public DbSet<Category> Categories { get; set; } = null!;
}