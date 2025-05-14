using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Skills;

public class Smite : Skill
{
    public Smite()
    {
        BaseSkillID = 4;
        ApplyValue = ApplyValue.New(5, 25, ApplyType.Damage, PropertyName.Life);
        Target = Target.New(TargetType.Select, TargetDirection.Enemy, 1);

        Costs.Add(Cost.New(10, CostType.Flat, PropertyName.Mana));
    }
}
