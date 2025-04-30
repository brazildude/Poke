namespace Poke.Core.Models.Skills;

public class Fireball : BaseSkill
{
    public Fireball()
    {
        SkillID = 1;
        SkillCost = ApplyValue.New(10, 10, ApplyType.Cost, ApplyToProperty.Mana);
        ApplyValue = ApplyValue.New(10, 15, ApplyType.Damage, ApplyToProperty.Life);
    }
}
