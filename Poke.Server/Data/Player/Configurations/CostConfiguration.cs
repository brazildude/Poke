using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Properties;

namespace Poke.Server.Data.Player.Configurations;

public class CostConfiguration : IEntityTypeConfiguration<Cost>
{
    public void Configure(EntityTypeBuilder<Cost> builder)
    {
        builder
            .HasOne(x => x.FlatProperty)
            .WithOne(x => x.Cost)
            .HasForeignKey<FlatProperty>(x => x.CostID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
           .Property(x => x.CostType)
           .HasConversion<string>();

        builder
           .Property(x => x.PropertyName)
           .HasConversion<string>();
    }
}
