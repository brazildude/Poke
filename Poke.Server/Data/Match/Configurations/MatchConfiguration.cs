using MemoryPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Match.Models;

namespace Poke.Server.Data.Match.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Models.Match>
{
    public void Configure(EntityTypeBuilder<Models.Match> builder)
    {
         builder.Property(m => m.MatchID)
            .ValueGeneratedNever();

        builder
           .Property(x => x.State)
           .HasConversion(
                v => MemoryPackSerializer.Serialize(v, null),
                v => MemoryPackSerializer.Deserialize<MatchState>(v, null)!);
    }
}
