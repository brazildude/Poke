using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Behavior
{
    public int BehaviorID { get; set; }
    public int SkillID { get; set; }
    public BehaviorName Name { get; set; }
    public BehaviorType Type { get; set; }

    public Target Target { get; set; } = null!;
    public Skill Skill { get; set; } = null!;

    public List<Cost> Costs { get; set; } = [];
    public List<FlatProperty> FlatProperties { get; set; } = [];
    public List<MinMaxProperty> MinMaxProperties { get; set; } = [];
}
