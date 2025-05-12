using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Configurations;
using Poke.Server.Data.Models;

namespace Poke.Server.Data;

public class PokeContext : DbContext
{
    public PokeContext(DbContextOptions<PokeContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Unit> BaseUnits => Set<Unit>();
    public DbSet<Skill> BaseSkills => Set<Skill>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UnitConfiguration).Assembly);
    }
}
