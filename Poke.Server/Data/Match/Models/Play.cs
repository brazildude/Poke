namespace Poke.Server.Data.Match.Models;

public class Play
{
    public int PlayID { get; set; }
    public int TeamID { get; set; }
    public int MatchID { get; set; }

    public int UnitInActionID { get; set; }
    public int SkillID { get; set; }
    public HashSet<int> TargetIDs { get; set; } = new HashSet<int>();

    //public Team Team { get; set; } = null!;
    public Match Match { get; set; } = null!;
}
