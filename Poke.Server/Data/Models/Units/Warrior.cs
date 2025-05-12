using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Warrior : Unit
{
    public Warrior()
    {
        BaseUnitID = 4;
        Name = typeof(Warrior).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<Skill>
        {
            new Cleave()
        };
    }
}
