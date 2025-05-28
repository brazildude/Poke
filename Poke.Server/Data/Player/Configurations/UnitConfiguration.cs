using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Data.Player.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder
            .HasMany(x => x.Skills)
            .WithOne(x => x.Unit)
            .HasForeignKey(x => x.UnitID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
           .HasMany(x => x.FlatProperties)
           .WithOne()
           .HasForeignKey(x => x.UnitID)
           .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.Name)
            .HasConversion<string>();
    }
}
