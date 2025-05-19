using Poke.Server.Data.Enums;
using Poke.Server.Infrastructure.Builders;

namespace Poke.Server.Data.Models.Skills;

public class Hellfire : Skill
{
    public Hellfire()
    {
        SkillName = SkillName.Hellfire;
        
        var behavior01 = CommonBehaviorBuilder.Create(BehaviorName.Hellfire01)
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
