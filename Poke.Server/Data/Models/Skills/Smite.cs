using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models.Skills;

public class Smite : Skill
{
    public Smite()
    {
        BaseSkillID = 4;
        Name = SkillName.Smite;
        
        Behaviors.Add(Behavior.New(-25, -5, ApplyType.Damage, PropertyName.Life, Target.New(TargetType.Select, TargetDirection.Enemy, 1)));
        Costs.Add(Cost.New(10, CostType.Flat, PropertyName.Mana));
        Properties.AddRange(FlatProperty.New(PropertyName.Cooldown, 0));
    }
}
