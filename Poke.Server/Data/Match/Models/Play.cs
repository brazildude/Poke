using MemoryPack;

namespace Poke.Server.Data.Match.Models;

[MemoryPackable]
public partial class Play
{
    public string UserID { get; set; } = null!;
    public int UnitInActionID { get; set; }
    public int SkillID { get; set; }
    public HashSet<int> TargetIDs { get; set; } = [];
    public DateTime PlayedAt { get; set; }
}
