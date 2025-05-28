using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Data.Player.Configurations;

public class TargetConfiguration : IEntityTypeConfiguration<Target>
{
    public void Configure(EntityTypeBuilder<Target> builder)
    {
        builder
           .Property(x => x.Type)
           .HasConversion<string>();

        builder
           .Property(x => x.Direction)
           .HasConversion<string>();

        builder
            .Property(x => x.TargetPropertyName)
            .HasConversion<string>();
    }
}
