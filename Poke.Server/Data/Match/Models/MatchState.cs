namespace Poke.Server.Data.Match.Models;

public class MatchState
{
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
}
