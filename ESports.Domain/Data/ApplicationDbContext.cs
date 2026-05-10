using ESports.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ESports.Domain.Data;

/// <summary>
/// Contexto da Base de Dados centralizado para a WebApp e WebApi.
/// </summary>
public class ApplicationDbContext : IdentityDbContext<MyUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<Match> Matches { get; set; } = null!;
    public DbSet<UserFavoriteTeam> UserFavoriteTeams { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

    }
}