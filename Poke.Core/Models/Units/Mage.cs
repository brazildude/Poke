
using Poke.Core.Models.Skills;

namespace Poke.Core.Models.Units;

public class Mage : BaseUnit
{
    public Mage()
    {
        UnitID = 1;
        Life = 100;
        Mana = 100;

        Skills = new List<BaseSkill>
        {
            new Fireball()
        };
    }
}
