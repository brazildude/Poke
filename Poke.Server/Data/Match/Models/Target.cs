using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class Target
{
    public TargetType Type { get; set; }
    public TargetDirection Direction { get; set; }
    public PropertyName TargetPropertyName { get; set; }
    public int? Quantity { get; set; } 
}
