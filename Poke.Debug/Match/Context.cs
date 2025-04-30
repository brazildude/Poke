namespace Poke.Debug.Match;

public class Context
{
    public int ContextID { get; set; }
    public int CurrentTeamID { get; set; }
    public int Round { get; set; }

    public required Team CurrentTeam { get; set; }
    public required Team NextTeam { get; set; }
}
