using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Skills;

public class Cleave : Skill
{
    public Cleave()
    {
        BaseSkillID = 1;
        ApplyValue = ApplyValue.New(5, 25, ApplyType.Damage, PropertyName.Life);
        Target = Target.New(TargetType.All, TargetDirection.Enemy, null);

        Costs.Add(Cost.New(10, CostType.Flat, PropertyName.Mana));
    }
}
