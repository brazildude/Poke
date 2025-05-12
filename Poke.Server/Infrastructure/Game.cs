using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Units;

namespace Poke.Server.Infrastructure;

public class Game
{
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
