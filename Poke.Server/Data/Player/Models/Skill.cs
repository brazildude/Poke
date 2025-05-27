using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Skill
{
    public int SkillID { get; set; }
    public int UnitID { get; set; }
    public SkillName SkillName { get; set; }

    public virtual List<Behavior> Behaviors { get; set; } = [];
    public virtual List<FlatProperty> Properties { get; set; } = [];
    
    public Unit Unit { get; set; } = null!;
}
