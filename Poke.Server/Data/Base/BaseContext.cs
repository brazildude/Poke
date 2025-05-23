using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Units;

namespace Poke.Server.Data.Base;

public class BaseContext
{
    public static List<Unit> GetUnits()
    {
        return
        [
           new Lancer(),
           new Mage(),
           new Paladin(),
           new Rogue(),
           new Warlock(),
           new Warrior()
        ];
    }

    public static Unit GetUnit(string unitName)
    {
        return unitName switch
        {
            nameof(Lancer) => new Lancer(),
            nameof(Mage) => new Mage(),
            nameof(Paladin) => new Paladin(),
            nameof(Rogue) => new Rogue(),
            nameof(Warlock) => new Warlock(),
            nameof(Warrior) => new Warrior(),
            _ => throw new ArgumentOutOfRangeException(nameof(unitName))
        };
    }
}
