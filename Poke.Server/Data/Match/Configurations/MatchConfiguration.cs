using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Poke.Server.Data.Match.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Models.Match>
{
    public void Configure(EntityTypeBuilder<Models.Match> builder)
    {
        builder.Ignore(x => x.State);
    }
}
