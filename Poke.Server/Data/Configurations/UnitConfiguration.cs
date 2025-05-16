using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Units;

namespace Poke.Server.Data.Configurations;

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
           .HasMany(x => x.Properties)
           .WithOne()
           .HasForeignKey(x => x.UnitID)
           .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.UnitName)
            .HasConversion<string>();

        builder.HasDiscriminator()
              .HasValue<Lancer>("Lancer")
              .HasValue<Mage>("Mage")
              .HasValue<Paladin>("Paladin")
              .HasValue<Rogue>("Rogue")
              .HasValue<Warlock>("Warlock")
              .HasValue<Warrior>("Warrior");
    }
}