using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ESports.Domain.Models;

namespace ESports.Domain.Data;

public class ApplicationDbContext : IdentityDbContext<MyUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<ESports.Domain.Models.Match> Matches { get; set; } = null!;
    public DbSet<UserFavoriteTeam> UserFavoriteTeams { get; set; } = null!;

    // O método tem de estar AQUI DENTRO, entre as chavetas da classe!
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