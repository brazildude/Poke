using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Configurations;

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
            .HasMany(x => x.Costs)
            .WithOne(x => x.Skill)
            .HasForeignKey(x => x.SkillID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Properties)
            .WithOne()
            .HasForeignKey(x => x.SkillID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
           .Property(x => x.SkillName)
           .HasConversion<string>();

        builder.HasDiscriminator()
               .HasValue<Cleave>("Cleave")
               .HasValue<DivineLight>("DivineLight")
               .HasValue<Fireball>("Fireball")
               .HasValue<Frostbolt>("Frostbolt")
               .HasValue<GlacialPuncture>("GlacialPuncture")
               .HasValue<Hellfire>("Hellfire")
               .HasValue<Lacerate>("Lacerate")
               .HasValue<Nullstep>("Nullstep")
               .HasValue<Shadowbolt>("Shadowbolt")
               .HasValue<Slice>("Slice")
               .HasValue<Smite>("Smite")
               .HasValue<SmokeMirage>("SmokeMirage");
    }
}
