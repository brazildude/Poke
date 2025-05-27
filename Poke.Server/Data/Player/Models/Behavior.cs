using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Behavior
{
    public int BehaviorID { get; set; }
    public int SkillID { get; set; }
    public BehaviorName BehaviorName { get; set; }
    public virtual BehaviorType BehaviorType { get; set; }

    public virtual Target Target { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;

    public virtual List<Cost> Costs { get; set; } = [];
    public virtual List<FlatProperty> Properties { get; set; } = [];
    public virtual List<MinMaxProperty> MinMaxProperties { get; set; } = [];
}
