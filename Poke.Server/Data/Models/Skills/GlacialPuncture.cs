using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models.Skills;

public class GlacialPuncture : Skill
{
    public GlacialPuncture()
    {
        SkillName = SkillName.GlacialPuncture;
        
        Behaviors.Add(Behavior.New(-25, -5, BehaviorType.Damage, PropertyName.Life, Target.New(TargetType.All, TargetDirection.Enemy, null)));
        Costs.Add(Cost.New(10, CostType.Flat, PropertyName.Mana));
        Properties.AddRange(FlatProperty.New(PropertyName.Cooldown, 0));
    }
}
