using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class Behavior
{
    public BehaviorName Name { get; set; }
    public BehaviorType Type { get; set; }

    public Target Target { get; set; } = null!;
    public List<Cost> Costs { get; set; } = [];
    public List<FlatProperty> FlatProperties { get; set; } = [];
    public List<MinMaxProperty> MinMaxProperties { get; set; } = [];
}
