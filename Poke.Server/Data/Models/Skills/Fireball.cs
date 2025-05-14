using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Skills;

public class Fireball : Skill
{
    public Fireball()
    {
        BaseSkillID = 2;
        Cost = Cost.New(10, CostType.Flat, ApplyToProperty.Mana);
        ApplyValue = ApplyValue.New(5, 25, ApplyType.Damage, ApplyToProperty.Life);
        Target = Target.New(TargetType.Select, TargetDirection.Enemy, 2);
    }
}
