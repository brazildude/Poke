using Poke.Server.Shared.Enums;
using Poke.Server.Infrastructure.Builders;

namespace Poke.Server.Data.Player.Models.Skills;

public class Slice
{
    public static Skill Create()
    {
        var behavior01 = BehaviorBuilder.Create(BehaviorName.Slice01)
            .WithTarget(TargetType.All, TargetDirection.Enemy, PropertyName.Life)
            .WithMinMax(PropertyName.BehaviorValue, 10, 20)
            .WithBehaviorType(BehaviorType.Damage)
            .WithCooldown(0)
            .WithCosts([Cost.New(10, CostType.Flat, PropertyName.Mana)])
            .Build();

        var behavior02 = BehaviorBuilder.Create(BehaviorName.Slice02)
            .WithTarget(TargetType.All, TargetDirection.Enemy, PropertyName.Life)
            .WithMinMax(PropertyName.BehaviorValue, 10, 20)
            .WithBehaviorType(BehaviorType.Damage)
            .WithCooldown(0)
            .WithCosts([Cost.New(10, CostType.Flat, PropertyName.Mana)])
            .Build();

        return new Skill
        {
            Name = SkillName.Slice,
            Behaviors = new List<Behavior> { behavior01, behavior02 },
        };
    }
}
