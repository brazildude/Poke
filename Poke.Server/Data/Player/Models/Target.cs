using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Target
{
    public int TargetID { get; set; }
    public int BehaviorID { get; set; }
    public TargetType TargetType { get; set; }
    public TargetDirection TargetDirection { get; set; }
    public PropertyName TargetPropertyName { get; set; }

    public int? Quantity { get; set; }

    public Behavior Behavior { get; set; } = null!;

    public static Target New(TargetType targetType, TargetDirection targetDirection, PropertyName targetPropertyName, int? quantity)
    {
        return new Target
        {
            TargetType = targetType,
            TargetDirection = targetDirection,
            TargetPropertyName = targetPropertyName,
            Quantity = quantity
        };
    }
}
