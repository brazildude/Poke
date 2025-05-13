using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.HasDiscriminator()
               .HasValue<Cleave>("Cleave")
               .HasValue<Fireball>("Fireball")
               .HasValue<Shadowbolt>("Shadowbolt")
               .HasValue<Cleave>("Cleave");
    }
}
