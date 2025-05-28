using Poke.Server.Data.Base;
using Poke.Server.Data.Match.Models;
using Poke.Server.Shared;
using Poke.Server.Shared.Enums;

namespace Poke.Tests.Infrastructure;

public class MatchGenerator
{
    public static Match CreateMatch()
    {
        var userID01 = "UserID01";
        var userID02 = "UserID02";

        return new Match
        {
            MatchID = Guid.NewGuid(),
            UserID01 = userID01,
            UserID02 = userID02,
            State = new MatchState
            {
                CurrentUserID = userID01,
                RandomSeed = Environment.TickCount,
                Round = 1,
                Plays = new List<Play>(),
                Teams = new Dictionary<string, Dictionary<int, Unit>> {
                    { userID01, CreateTeam01(0) },
                    { userID02, CreateTeam02(3) }
                }
            },
            IsMatchOver = false
        };
    }

    private static Dictionary<int, Unit> CreateTeam(int initialUnitID, params UnitName[] unitNames)
    {
        var units = unitNames
            .Select(unitName =>
            {
                var unit = BaseContext.GetUnit(unitName);
                unit.UnitID = ++initialUnitID;
                return unit;
            })
            .ToList();

        var data = units.AsQueryable();

        return Mapper.ToMatchTeam(data).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<int, Unit> CreateTeam01(int initialUnitID)
    {
        return CreateTeam(initialUnitID, UnitName.Warrior, UnitName.Mage, UnitName.Paladin, UnitName.Rogue);
    }

    private static Dictionary<int, Unit> CreateTeam02(int initialUnitID)
    {
        return CreateTeam(initialUnitID, UnitName.Mage, UnitName.Warlock, UnitName.Paladin, UnitName.Rogue);
    }
}
