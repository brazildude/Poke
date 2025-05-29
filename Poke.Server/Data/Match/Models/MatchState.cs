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

    public List<Play> Plays { private get; set; } = [];
    public Dictionary<string, Dictionary<int, Unit>> Teams { get; set; } = [];

    [MemoryPackIgnore]
    public Play CurrentPlay { private get; set; } = null!;

    public Dictionary<int, Unit> GetCurrentTeam()
    {
        return Teams[CurrentUserID];
    }

    public Dictionary<int, Unit> GetEnemyTeam()
    {
        var enemyUserID = Teams.Keys.Single(x => x != CurrentUserID);
        return Teams[enemyUserID];
    }

    public void AddPlay(Play play)
    {
        CurrentPlay = play;
        Plays.Add(play);
    }

    public void AddEvent(GameEvent gameEvent)
    {
        if (CurrentPlay == null)
        {
            throw new InvalidOperationException("Current play is not set. Cannot add event.");
        }

        CurrentPlay.Events.Add(gameEvent);
    }

    public List<GameEvent> GetTurnEvents()
    {
        if (CurrentPlay == null)
        {
            throw new InvalidOperationException("Current play is not set. Cannot get turn events.");
        }

        return CurrentPlay.Events;
    }
}
