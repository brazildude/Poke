using Poke.Server.Data.Match.Models;
using Poke.Server.GameLogic.Events;

namespace Poke.Server.Shared.Extensions;

public static class EventExtensions
{
    public static void AddUnitSelectedEvent(this MatchState matchState, int unitID)
    {
        var e = new UnitSelectedEvent { Type = nameof(UnitSelectedEvent), UnitID = unitID };
        matchState.AddEvent(e);
    }

    public static void AddSkillSelectedEvent(this MatchState matchState, int skillID)
    {
        var e = new SkillSelectedEvent { Type = nameof(SkillSelectedEvent), SkillID = skillID };
        matchState.AddEvent(e);
    }
}
