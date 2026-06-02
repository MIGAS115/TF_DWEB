using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Models;

namespace ESports.Domain.Data
{
    /// <summary>
    /// Contexto principal da base de dados responsável pelo mapeamento das entidades e gestão do sistema de autenticação Identity.
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
        /// Tabela representativa das equipas de e-sports.
        /// </summary>
        public DbSet<Team> Teams { get; set; } = null!;

        /// <summary>
        /// Tabela representativa dos jogos (Matches) agendados ou concluídos.
        /// </summary>
        public DbSet<ESports.Domain.Models.Match> Matches { get; set; } = null!;

        /// <summary>
        /// Tabela intermédia de associação N:M para registo das equipas favoritas de cada utilizador.
        /// </summary>
        public DbSet<Favorite> Favorites { get; set; } = null!;

        /// <summary>
        /// Tabela representativa dos utilizadores com privilégios de Administração.
        /// </summary>
        public DbSet<Admin> Admins { get; set; } = null!;

        /// <summary>
        /// Tabela representativa dos utilizadores regulares/normais da plataforma.
        /// </summary>
        public DbSet<RegularUser> RegularUsers { get; set; } = null!;

        /// <summary>
        /// Tabela representativa dos torneios e competições.
        /// </summary>
        public DbSet<Tournament> Tournaments { get; set; } = null!;

        /// <summary>
        /// Tabela intermédia de associação N:M para mapeamento das equipas inscritas em cada torneio.
        /// </summary>
        public DbSet<TournamentTeam> TournamentTeams { get; set; } = null!;

        /// <summary>
        /// Configuração das regras globais e restrições do modelo de dados.
        /// </summary>
        /// <param name="builder">ModelBuilder utilizado pelo Entity Framework Core.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var entidadesDominio = builder.Model.GetEntityTypes()
                .Where(e => e.ClrType.Namespace != null && e.ClrType.Namespace.StartsWith("ESports.Domain.Models"));

            foreach (var relationship in entidadesDominio.SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}