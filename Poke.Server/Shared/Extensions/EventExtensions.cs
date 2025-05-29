using Poke.Server.Data.Match.Models;
using Poke.Server.GameLogic.Events;
using Poke.Server.Shared.Enums;

namespace Poke.Server.Shared.Extensions;

public static class EventExtensions
{
    public static void AddUnitStateChangedEvent(
        this MatchState matchState,
        string type,
        int unitID,
        string propertyName,
        int appliedValud,
        int currentValue,
        HitType hitType)
    {
        var e = new UnitStateChangedEvent
        {
            Type = type,
            UnitID = unitID,
            PropertyName = propertyName,
            AppliedValue = appliedValud,
            CurrentValue = currentValue,
            HitType = hitType
        };
        matchState.AddEvent(e);
    }
}
