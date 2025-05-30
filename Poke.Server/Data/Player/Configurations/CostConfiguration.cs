using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;

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
           .Property(x => x.Type)
           .HasConversion<string>();

        builder
           .Property(x => x.CostPropertyName)
           .HasConversion<string>();
    }
}
