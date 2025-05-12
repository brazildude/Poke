using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Paladin : Unit
{
    public Paladin()
    {
        BaseUnitID = 2;
        Name = typeof(Paladin).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<Skill>
        {
            new Smite()
        };
    }
}
