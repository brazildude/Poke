using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Units;

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
           .HasMany(x => x.Properties)
           .WithOne()
           .HasForeignKey(x => x.UnitID)
           .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(x => x.UnitName)
            .HasConversion<string>();

        // mapping all skills to the database
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && typeof(Unit).IsAssignableFrom(type))
            .ToList();

        var discriminator = builder.HasDiscriminator();

        foreach (var type in types)
        {
            discriminator.HasValue(type, type.Name);
        }
    }
}
