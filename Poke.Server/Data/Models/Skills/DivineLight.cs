using Poke.Server.Data.Enums;
using Poke.Server.Infrastructure.Builders;

namespace Poke.Server.Data.Models.Skills;

public class DivineLight : Skill
{
    public DivineLight()
    {
        SkillName = SkillName.DivineLight;
        
        var behavior01 = CommonBehaviorBuilder.Create(BehaviorName.DivineLight01)
                    .WithTarget(TargetType.All, TargetDirection.Enemy)
                    .WithMinMax(PropertyName.BehaviorValue, 10, 20)
                    .WithBehaviorType(BehaviorType.Damage)
                    .WithPropertyName(PropertyName.Life)
                    .WithCooldown(0)
                    .WithCosts(new List<Cost> { Cost.New(10, CostType.Flat, PropertyName.Mana) })
                    .Build();

        Behaviors.Add(behavior01);
    }
}
