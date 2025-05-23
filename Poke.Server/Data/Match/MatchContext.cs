using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Match.Configurations;
using Poke.Server.Data.Match.Models;

namespace Poke.Server.Data.Match;

public class MatchContext : DbContext
{
    public MatchContext(DbContextOptions<MatchContext> options) : base(options) { }

    public DbSet<Models.Match> Matches => Set<Models.Match>();
    public DbSet<Play> Plays => Set<Play>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("match");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlayConfiguration).Assembly, x => x.FullName!.Contains(".Match.Configurations"));

        if (Database.IsSqlite())
        {
            modelBuilder.Entity<Models.Match>().ToTable("match_matches");
            modelBuilder.Entity<Play>().ToTable("match_plays");
        }
    }
}
