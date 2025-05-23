using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Properties;
using Poke.Server.Data.Player.Models.Skills;

namespace Poke.Server.Data.Player.Models.Units;

public class Lancer : Unit
{
    public Lancer()
    {
        UnitName = UnitName.Lancer;

        Skills.AddRange(
            new Slice(),
            new GlacialPuncture()
        );

        Properties.AddRange(
            FlatProperty.New(PropertyName.Life, 100),
            FlatProperty.New(PropertyName.Mana, 100),
            FlatProperty.New(PropertyName.PlayTimes, 1)
        );
    }
}
