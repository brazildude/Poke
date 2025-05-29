using MemoryPack;
using Poke.Server.GameLogic.Events;

namespace Poke.Server.Data.Match.Models;

[MemoryPackable]
public partial class MatchState
{
    public Guid MatchID { get; set; }
    public string CurrentUserID { get; set; } = null!;
    public string EnemyUserID { get; set; } = null!;
    public int RandomSeed { get; set; }
    public int Round { get; set; }
    public int LastEventID { get; set; } = 0;

    public List<Play> Plays { get; set; } = [];
    public Dictionary<string, Dictionary<int, Unit>> Teams { get; set; } = [];
    public List<GameEvent> Events { get; set; } = new();

    [MemoryPackIgnore]
    public List<GameEvent> TurnEvents { get; set; } = [];

    public Dictionary<int, Unit> GetCurrentTeam()
    {
        return Teams[CurrentUserID];
    }

    public Dictionary<int, Unit> GetEnemyTeam()
    {
        var enemyUserID = Teams.Keys.Single(x => x != CurrentUserID);
        return Teams[enemyUserID];
    }

    public void AddEvent(GameEvent evt)
    {
        evt.EventId = ++LastEventID;
        TurnEvents.Add(evt);
    }

    public List<GameEvent> GetEventsSince(int lastSeenId)
    {
        return Events.Where(e => e.EventId > lastSeenId).ToList();
    }

    public List<GameEvent> GetTurnEvents()
    {
        Events.AddRange(TurnEvents);
        return TurnEvents;
    }
}
