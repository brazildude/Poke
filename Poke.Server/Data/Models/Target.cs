using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public class Target
{
    public int TargetID { get; set; }
    public int BehaviorID { get; set; }
    public TargetType TargetType { get; set; }
    public TargetDirection TargetDirection { get; set; }
    public int? Quantity { get; set; }

    public Behavior Behavior { get; set; } = null!;

    public static Target New(TargetType targetType, TargetDirection targetDirection, int? quantity)
    {
        return new Target
        {
            TargetType = targetType,
            TargetDirection = targetDirection,
            Quantity = quantity
        };
    }
}
