using MemoryPack;
using Poke.Server.GameLogic.Events;

namespace Poke.Server.Data.Match.Models;

[MemoryPackable]
public partial class Play
{
    public long PlayedAt { get; init; } = DateTime.UtcNow.Ticks;
    public string UserID { get; set; } = null!;
    public int UnitInActionID { get; set; }
    public int SkillID { get; set; }
    public HashSet<int> TargetIDs { get; set; } = [];
    public List<GameEvent> Events { get; set; } = new();
}
