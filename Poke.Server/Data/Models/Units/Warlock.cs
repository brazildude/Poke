using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Warlock : BaseUnit
{
    public Warlock()
    {
        Name = typeof(Warlock).Name;
        Life = 100;
        Mana = 100;

        Skills = new List<BaseSkill>
        {
            new Shadowbolt()
        };
    }
}
