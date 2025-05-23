using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Behaviors;
using Poke.Server.Data.Player.Models.Properties;

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
            .HasMany(x => x.MinMaxProperties)
            .WithOne(x => x.Behavior)
            .HasForeignKey(x => x.BehaviorID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.BehaviorType)
            .HasConversion<string>();

        builder.HasDiscriminator()
               .HasValue<CommonBehavior>("CommonBehavior");
    }
}
