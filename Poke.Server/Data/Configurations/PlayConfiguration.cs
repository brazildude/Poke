using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Models;

namespace Poke.Server.Data.Configurations;

public class PlayConfiguration : IEntityTypeConfiguration<Play>
{
    public void Configure(EntityTypeBuilder<Play> builder)
    {
        builder
            .Property(x => x.TargetIDs)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<HashSet<int>>(v, (JsonSerializerOptions)null!)!
            );
    }
}
