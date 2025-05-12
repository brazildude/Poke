using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Warlock : Unit
{
    public Warlock()
    {
        BaseUnitID = 3;
        Name = typeof(Warlock).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<Skill>
        {
            new Shadowbolt()
        };
    }
}
