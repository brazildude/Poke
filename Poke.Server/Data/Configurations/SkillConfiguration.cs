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
            .HasMany(x => x.Properties)
            .WithOne()
            .HasForeignKey(x => x.SkillID)
            .OnDelete(DeleteBehavior.Cascade);

        builder
           .Property(x => x.SkillName)
           .HasConversion<string>();

        // mapping all skills to the database
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && typeof(Skill).IsAssignableFrom(type))
            .ToList();

        var discriminator = builder.HasDiscriminator();

        foreach (var type in types)
        {
            discriminator.HasValue(type, type.Name);
        }
    }
}
