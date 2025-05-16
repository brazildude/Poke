using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Rogue : Unit
{
    public Rogue()
    {
        UnitName = UnitName.Rogue;

        Skills.AddRange(
            new Nullstep(),
            new SmokeMirage()
        );
        
        Properties.AddRange(
            FlatProperty.New(PropertyName.Life, 100),
            FlatProperty.New(PropertyName.Mana, 100)
        );
    }
}
