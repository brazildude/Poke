using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Configurations;
using Poke.Server.Data.Models;

namespace Poke.Server.Data;

public class PokeDbContext : DbContext
{
    public PokeDbContext(DbContextOptions<PokeDbContext> options) : base(options) { }

    public DbSet<Behavior> Bahaviors => Set<Behavior>();
    public DbSet<Cost> Costs => Set<Cost>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Target> Targets => Set<Target>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UnitConfiguration).Assembly);
    }
}
