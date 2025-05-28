using Poke.Server.Shared.Enums;
using Poke.Server.Infrastructure.Builders;

namespace Poke.Server.Data.Player.Models.Skills;

public class Smite
{
    public static Skill Create()
    {
        var behavior01 = BehaviorBuilder.Create(BehaviorName.Smite01)
            .WithTarget(TargetType.All, TargetDirection.Enemy, PropertyName.Life)
            .WithMinMax(PropertyName.BehaviorValue, 10, 20)
            .WithBehaviorType(BehaviorType.Damage)
            .WithCooldown(0)
            .WithCosts([Cost.New(10, CostType.Flat, PropertyName.Mana)])
            .Build();

        return new Skill
        {
            Name = SkillName.Smite,
            Behaviors = new List<Behavior> { behavior01 },
        };
    }
}
