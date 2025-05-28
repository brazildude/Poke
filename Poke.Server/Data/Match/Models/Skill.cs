using MemoryPack;
using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

[MemoryPackable]
public partial class Skill
{
    public int SkillID { get; set; }
    public SkillName Name { get; set; }

    public List<Behavior> Behaviors { get; set; } = [];
    public List<FlatProperty> FlatProperties { get; set; } = [];
}
