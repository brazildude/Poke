using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Properties;
using Poke.Server.Data.Player.Models.Skills;

namespace Poke.Server.Data.Player.Models.Units;

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
            FlatProperty.New(PropertyName.Mana, 100),
            FlatProperty.New(PropertyName.PlayTimes, 1)
        );
    }
}
