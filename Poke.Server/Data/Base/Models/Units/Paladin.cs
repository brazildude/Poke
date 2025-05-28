using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Skills;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Data.Base.Models.Units;

public class Paladin
{
    public static Unit Create()
    {
        return new Unit
        {
            Name = UnitName.Paladin,
            Skills = new()
            {
                Smite.Create(),
                DivineLight.Create()
            },
            FlatProperties = new()
            {
                FlatProperty.New(PropertyName.Life, 100),
                FlatProperty.New(PropertyName.Mana, 100),
                FlatProperty.New(PropertyName.PlayTimes, 1)
            }
        };
    }
}
