using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Units;

namespace Poke.Server.Data.Configurations;

public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        builder.HasDiscriminator()
              .HasValue<Mage>("Mage")
              .HasValue<Paladin>("Paladin")
              .HasValue<Warlock>("Warlock")
              .HasValue<Warrior>("Warrior");

        builder.HasIndex(x => x.BaseUnitID).IsUnique();
    }
}