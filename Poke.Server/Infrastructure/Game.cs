using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Units;

namespace Poke.Server.Infrastructure;

public class Game
{
    public static List<Unit> GetUnits()
    {
        return new List<Unit>
        {
           new Lancer(),
           new Mage(),
           new Paladin(),
           new Rogue(),
           new Warlock(),
           new Warrior()
        };
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
