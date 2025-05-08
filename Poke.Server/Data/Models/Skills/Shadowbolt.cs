using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Skills;

public class Shadowbolt : BaseSkill
{
    public Shadowbolt()
    {
        SkillCost = ApplyValue.New(10, 10, ApplyType.Cost, ApplyToProperty.Mana);
        ApplyValue = ApplyValue.New(5, 25, ApplyType.Damage, ApplyToProperty.Life);
    }
}
