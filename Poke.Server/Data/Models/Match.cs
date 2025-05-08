namespace Poke.Server.Data.Models;

public class Match
{
    public int MatchID { get; set; }
    public int CurrentTeamID { get; set; }
    public int Round { get; set; }

    public required Team CurrentTeam { get; set; }
    public required Team NextTeam { get; set; }
}
