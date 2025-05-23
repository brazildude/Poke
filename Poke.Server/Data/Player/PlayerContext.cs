using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player.Configurations;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Properties;

namespace Poke.Server.Data.Player;

public class PlayerContext : DbContext
{
    public PlayerContext(DbContextOptions<PlayerContext> options) : base(options) { }

    public DbSet<Behavior> Bahaviors => Set<Behavior>();
    public DbSet<Cost> Costs => Set<Cost>();
    public DbSet<FlagProperty> FlagProperties => Set<FlagProperty>();
    public DbSet<FlatProperty> FlatProperties => Set<FlatProperty>();
    public DbSet<MinMaxProperty> MinMaxProperties => Set<MinMaxProperty>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Target> Targets => Set<Target>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("player");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BehaviorConfiguration).Assembly, x => x.FullName!.Contains(".Player.Configurations"));

        if (Database.IsSqlite())
        {
            modelBuilder.Entity<Behavior>().ToTable("player_behaviors");
            modelBuilder.Entity<Cost>().ToTable("player_costs");
            modelBuilder.Entity<FlagProperty>().ToTable("player_flag_properties");
            modelBuilder.Entity<FlatProperty>().ToTable("player_flat_properties");
            modelBuilder.Entity<MinMaxProperty>().ToTable("player_minmax_properties");
            modelBuilder.Entity<Skill>().ToTable("player_skills");
            modelBuilder.Entity<Target>().ToTable("player_targets");
            modelBuilder.Entity<Team>().ToTable("player_teams");
            modelBuilder.Entity<Unit>().ToTable("player_units");
            modelBuilder.Entity<User>().ToTable("player_users");
        }
    }
}
