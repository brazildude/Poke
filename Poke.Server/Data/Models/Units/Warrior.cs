using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Warrior : Unit
{
    public Warrior()
    {
        BaseUnitID = 4;
        Name = typeof(Warrior).Name;

        Skills.Add(new Cleave());
        Properties.AddRange(
            FlatProperty.New(PropertyName.Life, 100),
            FlatProperty.New(PropertyName.Mana, 100)
        );
    }
}
