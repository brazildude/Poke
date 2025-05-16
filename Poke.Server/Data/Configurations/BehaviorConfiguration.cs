using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Configurations;

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
            .HasOne(x => x.MinMaxProperty)
            .WithOne(x => x.Behavior)
            .HasForeignKey<MinMaxProperty>(x => x.BehaviorID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
