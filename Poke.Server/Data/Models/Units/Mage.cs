using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Mage : Unit
{
    public Mage()
    {
        BaseUnitID = 1;
        Name = typeof(Mage).Name;

        Skills.Add(new Fireball());
        Properties.AddRange(
            FlatProperty.New(PropertyName.Life, 100),
            FlatProperty.New(PropertyName.Mana, 100)
        );
    }
}
