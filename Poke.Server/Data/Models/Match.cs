namespace Poke.Server.Data.Models;

public class Match
{
    public int MatchID { get; set; }
    public int User01ID { get; set; }
    public int User02ID { get; set; }
    public int CurrentTeamID { get; set; }
    public int Round { get; set; }

    public Team CurrentTeam { get; set; } = null!;
    public Team NextTeam { get; set; } = null!;
    public User User01 { get; set; } = null!;
    public User User02 { get; set; } = null!;
}
