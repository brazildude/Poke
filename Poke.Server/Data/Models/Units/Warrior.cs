using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Warrior : BaseUnit
{
    public Warrior()
    {
        Name = typeof(Warrior).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<BaseSkill>
        {
            new Cleave()
        };
    }
}
