using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Mage : BaseUnit
{
    public Mage()
    {
        Name = typeof(Mage).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<BaseSkill>
        {
            new Fireball()
        };
    }
}
