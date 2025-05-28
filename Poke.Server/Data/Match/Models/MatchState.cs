using System.Collections.Concurrent;
using MemoryPack;
using Poke.Server.GameLogic.Events;

namespace Poke.Server.Data.Match.Models;

[MemoryPackable]
public partial class MatchState
{
    [MemoryPackInclude]
    private ConcurrentQueue<GameEvent> _eventQueue = new();
    
    [MemoryPackInclude]
    private int _nextEventId = 1;

    public string CurrentUserID { get; set; } = null!;
    public int RandomSeed { get; set; }
    public int Round { get; set; }

    public List<Play> Plays { get; set; } = [];
    public Dictionary<string, Dictionary<int, Unit>> Teams { get; set; } = [];

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
        evt.EventId = Interlocked.Increment(ref _nextEventId);
        _eventQueue.Enqueue(evt);
    }

    public List<GameEvent> GetEventsSince(int lastSeenId)
    {
        return _eventQueue.Where(e => e.EventId > lastSeenId).ToList();
    }
}
