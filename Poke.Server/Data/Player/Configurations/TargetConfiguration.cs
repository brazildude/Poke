using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Data.Player.Configurations;

public class TargetConfiguration : IEntityTypeConfiguration<Target>
{
    public void Configure(EntityTypeBuilder<Target> builder)
    {
        builder
           .Property(x => x.TargetType)
           .HasConversion<string>();

        builder
           .Property(x => x.TargetDirection)
           .HasConversion<string>();

        builder
            .Property(x => x.TargetPropertyName)
            .HasConversion<string>();
    }
}
