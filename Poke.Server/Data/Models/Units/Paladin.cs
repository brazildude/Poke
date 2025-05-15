using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

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
            FlatProperty.New(PropertyName.Mana, 100)
        );
    }
}
