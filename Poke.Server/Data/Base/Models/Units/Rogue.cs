using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Skills;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Data.Base.Models.Units;

public class Rogue
{
    public static Unit Create()
    {
        return new Unit
        {
            UnitName = UnitName.Rogue,
            Skills =
            [
                Nullstep.Create(),
                SmokeMirage.Create()
            ],
            Properties =
            [
                FlatProperty.New(PropertyName.Life, 100),
                FlatProperty.New(PropertyName.Mana, 100),
                FlatProperty.New(PropertyName.PlayTimes, 1)
            ]
        };
    }
}
