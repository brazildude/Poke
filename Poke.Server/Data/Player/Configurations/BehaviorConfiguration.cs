using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Data.Player.Configurations;

public class BehaviorConfiguration : IEntityTypeConfiguration<Behavior>
{
    public void Configure(EntityTypeBuilder<Behavior> builder)
    {
        builder
            .HasOne(x => x.Target)
            .WithOne(x => x.Behavior)
            .HasForeignKey<Target>(x => x.BehaviorID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Costs)
            .WithOne(x => x.Behavior)
            .HasForeignKey(x => x.BehaviorID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.FlatProperties)
            .WithOne(x => x.Behavior)
            .HasForeignKey(x => x.BehaviorID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.MinMaxProperties)
            .WithOne(x => x.Behavior)
            .HasForeignKey(x => x.BehaviorID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.Type)
            .HasConversion<string>();
    }
}
