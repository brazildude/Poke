using Poke.Server.Shared.Enums;
using Poke.Server.Infrastructure.Builders;

namespace Poke.Server.Data.Player.Models.Skills;

public class Cleave : Skill
{
    public Cleave()
    {
        SkillName = SkillName.Cleave;
        
        var behavior01 = CommonBehaviorBuilder.Create(BehaviorName.Cleave01)
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