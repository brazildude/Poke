using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Match.Configurations;
using Poke.Server.Data.Match.Models;

namespace Poke.Server.Data.Match;

public class MatchContext : DbContext
{
    public MatchContext(DbContextOptions<MatchContext> options) : base(options) { }

    public DbSet<Models.Match> Matches => Set<Models.Match>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("match");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MatchConfiguration).Assembly, x => x.FullName!.Contains(".Match.Configurations"));

        if (Database.IsSqlite())
        {
            modelBuilder.Entity<Models.Match>().ToTable("match_matches");
        }
    }
}
