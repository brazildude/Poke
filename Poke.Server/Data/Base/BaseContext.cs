using Poke.Server.Data.Base.Models.Units;
using Poke.Server.Data.Player.Models;
using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Base;

public class BaseContext
{
    private static Dictionary<UnitName, Unit> map = [];

    static BaseContext()
    {
        map.Add(UnitName.Lancer, Lancer.Create());
        map.Add(UnitName.Mage, Mage.Create());
        map.Add(UnitName.Paladin, Paladin.Create());
        map.Add(UnitName.Rogue, Rogue.Create());
        map.Add(UnitName.Warlock, Warlock.Create());
        map.Add(UnitName.Warrior, Warrior.Create());
    }

    public static List<Unit> GetUnits()
    {
        return
        [
            Lancer.Create(),
            Mage.Create(),
            Paladin.Create(),
            Rogue.Create(),
            Warlock.Create(),
            Warrior.Create()
        ];
    }

    public static Unit GetUnit(UnitName unitName)
    {
        return unitName switch
        {
            UnitName.Lancer => Lancer.Create(),
            UnitName.Mage => Mage.Create(),
            UnitName.Paladin => Paladin.Create(),
            UnitName.Rogue => Rogue.Create(),
            UnitName.Warlock => Warlock.Create(),
            UnitName.Warrior => Warrior.Create(),
            _ => throw new ArgumentOutOfRangeException(nameof(unitName))
        };
    }
}
