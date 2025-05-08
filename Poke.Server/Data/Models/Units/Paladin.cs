using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Paladin : BaseUnit
{
    public Paladin()
    {
        Name = typeof(Paladin).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<BaseSkill>
        {
            new Smite()
        };
    }
}
