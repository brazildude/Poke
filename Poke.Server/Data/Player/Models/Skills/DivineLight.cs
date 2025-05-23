using Poke.Server.Shared.Enums;
using Poke.Server.Infrastructure.Builders;

namespace Poke.Server.Data.Player.Models.Skills;

public class DivineLight : Skill
{
    public DivineLight()
    {
        SkillName = SkillName.DivineLight;
        
        var behavior01 = CommonBehaviorBuilder.Create(BehaviorName.DivineLight01)
                    .WithTarget(TargetType.All, TargetDirection.Enemy, PropertyName.Life)
                    .WithMinMax(PropertyName.BehaviorValue, 10, 20)
                    .WithBehaviorType(BehaviorType.Damage)
                    .WithCooldown(0)
                    .WithCosts([Cost.New(10, CostType.Flat, PropertyName.Mana)])
                    .Build();

        Behaviors.Add(behavior01);
    }
}
