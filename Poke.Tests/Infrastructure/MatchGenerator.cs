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

    private static Dictionary<int, Unit> CreateTeam01(int initialUnitID)
    {
        var unit01 = BaseContext.GetUnit(UnitName.Warrior);
        var unit02 = BaseContext.GetUnit(UnitName.Mage);
        var unit03 = BaseContext.GetUnit(UnitName.Paladin);
        var unit04 = BaseContext.GetUnit(UnitName.Rogue);

        unit01.UnitID = ++initialUnitID;
        unit02.UnitID = ++initialUnitID;
        unit03.UnitID = ++initialUnitID;
        unit04.UnitID = ++initialUnitID;

        var data = new List<Server.Data.Player.Models.Unit>
        {
            unit01,
            unit02,
            unit03,
            unit04
        }.AsQueryable();

        return Mapper.ToMatchTeam(data).ToDictionary(x => x.Key, x => x.Value);
    }

    private static Dictionary<int, Unit> CreateTeam02(int initialUnitID)
    {
        var unit01 = BaseContext.GetUnit(UnitName.Mage);
        var unit02 = BaseContext.GetUnit(UnitName.Warlock);
        var unit03 = BaseContext.GetUnit(UnitName.Paladin);
        var unit04 = BaseContext.GetUnit(UnitName.Rogue);

        unit01.UnitID = ++initialUnitID;
        unit02.UnitID = ++initialUnitID;
        unit03.UnitID = ++initialUnitID;
        unit04.UnitID = ++initialUnitID;

        var data = new List<Server.Data.Player.Models.Unit>
        {
            unit01,
            unit02,
            unit03,
            unit04
        }.AsQueryable();

        return Mapper.ToMatchTeam(data).ToDictionary(x => x.Key, x => x.Value);
    }
}
