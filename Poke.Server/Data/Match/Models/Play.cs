namespace Poke.Server.Data.Match.Models;

public class Play
{
    public string UserID { get; set; } = null!;
    public int UnitInActionID { get; set; }
    public int SkillID { get; set; }
    public HashSet<int> TargetIDs { get; set; } = [];
    public DateTimeOffset PlayedAt { get; set; }
}
