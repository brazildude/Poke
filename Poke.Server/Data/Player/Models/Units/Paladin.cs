using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Properties;
using Poke.Server.Data.Player.Models.Skills;

namespace Poke.Server.Data.Player.Models.Units;

public class Paladin : Unit
{
    public Paladin()
    {
        UnitName = UnitName.Paladin;

        Skills.AddRange(
            new Smite(),
            new DivineLight()
        );

        Properties.AddRange(
            FlatProperty.New(PropertyName.Life, 100),
            FlatProperty.New(PropertyName.Mana, 100),
            FlatProperty.New(PropertyName.PlayTimes, 1)
        );
    }
}
