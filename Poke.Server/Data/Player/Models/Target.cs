using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Target
{
    public int TargetID { get; set; }
    public int BehaviorID { get; set; }
    public TargetType Type { get; set; }
    public TargetDirection Direction { get; set; }
    public PropertyName TargetPropertyName { get; set; }
    public int? Quantity { get; set; }

    public Behavior Behavior { get; set; } = null!;

    public static Target New(TargetType type, TargetDirection direction, PropertyName targetPropertyName, int? quantity)
    {
        return new Target
        {
            Type = type,
            Direction = direction,
            TargetPropertyName = targetPropertyName,
            Quantity = quantity
        };
    }
}
