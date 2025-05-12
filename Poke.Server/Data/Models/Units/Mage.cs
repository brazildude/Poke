using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Mage : Unit
{
    public Mage()
    {
        BaseUnitID = 1;
        Name = typeof(Mage).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<Skill>
        {
            new Fireball()
        };
    }
}
