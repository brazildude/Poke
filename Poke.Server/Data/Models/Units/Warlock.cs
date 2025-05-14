using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Data.Models.Skills;

namespace Poke.Server.Data.Models.Units;

public class Warlock : Unit
{
    public Warlock()
    {
        BaseUnitID = 3;
        Name = typeof(Warlock).Name;
       
        Skills.Add(new Shadowbolt());
        Properties.AddRange(
            FlatProperty.New(PropertyName.Life, 100),
            FlatProperty.New(PropertyName.Mana, 100)
        );
    }
}
