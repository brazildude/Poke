using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Skill
{
    public int SkillID { get; set; }
    public int UnitID { get; set; }
    public SkillName Name { get; set; }

    public virtual List<Behavior> Behaviors { get; set; } = [];
    public virtual List<FlatProperty> FlatProperties { get; set; } = [];
    
    public Unit Unit { get; set; } = null!;
}
