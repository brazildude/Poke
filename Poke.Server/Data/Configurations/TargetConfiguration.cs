using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Configurations;

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
    }
}
