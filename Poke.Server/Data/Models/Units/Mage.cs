using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Mage : Unit
{
    public Mage()
    {
        UnitName = UnitName.Mage;

        Skills.AddRange(
            new Fireball(),
            new Frostbolt()
        );
        
        Properties.AddRange(
            FlatProperty.New(PropertyName.Life, 100),
            FlatProperty.New(PropertyName.Mana, 100),
            FlatProperty.New(PropertyName.PlayTimes, 1)
        );
    }
}
