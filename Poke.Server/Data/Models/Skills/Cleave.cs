using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Skills;

public class Cleave : Skill
{
    public Cleave()
    {
        BaseSkillID = 1;
        SkillCost = ApplyValue.New(10, 10, ApplyType.Cost, ApplyToProperty.Mana);
        ApplyValue = ApplyValue.New(5, 25, ApplyType.Damage, ApplyToProperty.Life);
    }
}
