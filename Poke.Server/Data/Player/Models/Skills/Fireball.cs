using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Behaviors;
using Poke.Server.Infrastructure.Builders;

namespace Poke.Server.Data.Player.Models.Skills;

public class Fireball : Skill
{
    public Fireball()
    {
        SkillName = SkillName.Fireball;
        
        var behavior01 = CommonBehaviorBuilder.Create(BehaviorName.Fireball01)
                    .WithTarget(TargetType.All, TargetDirection.Enemy, PropertyName.Life)
                    .WithMinMax(PropertyName.BehaviorValue, -20, -10)
                    .WithBehaviorType(BehaviorType.Damage)
                    .WithCooldown(0)
                    .WithCosts([Cost.New(-10, CostType.Flat, PropertyName.Mana)])
                    .Build();

        Behaviors.Add(behavior01);
        Behaviors.Add(new FireballBehavior02());
    }
}
