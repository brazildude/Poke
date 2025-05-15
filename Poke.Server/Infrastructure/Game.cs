using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Units;

namespace Poke.Server.Infrastructure;

public class Game
{
    public static List<Unit> GetUnits()
    {
        return new List<Unit>
        {
           new Mage(),
           new Paladin(),
           new Warlock(),
           new Warrior()
        };
    }

    public static Unit GetUnit(int baseUnitID)
    {
        return baseUnitID switch
        {
            1 => new Mage(),
            2 => new Paladin(),
            3 => new Warlock(),
            4 => new Warrior(),
            _ => throw new ArgumentOutOfRangeException(nameof(baseUnitID))
        };
    }
}
