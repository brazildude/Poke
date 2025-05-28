using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Data.Player.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder
            .HasMany(x => x.Behaviors)
            .WithOne(x => x.Skill)
            .HasForeignKey(x => x.SkillID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.FlatProperties)
            .WithOne()
            .HasForeignKey(x => x.SkillID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
           .Property(x => x.Name)
           .HasConversion<string>();
    }
}
