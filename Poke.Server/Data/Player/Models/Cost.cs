using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Cost
{
    public int CostID { get; set; }
    public int BehaviorID { get; set; }
    public CostType Type { get; set; }
    public PropertyName CostPropertyName { get; set; }

    public FlatProperty FlatProperty { get; set; } = null!;
    public Behavior Behavior { get; set; } = null!;

    public static Cost New(int value, CostType type, PropertyName costPropertyName)
    {
        return new Cost
        {
            FlatProperty = FlatProperty.New(PropertyName.SkillCost, value),
            Type = type,
            CostPropertyName = costPropertyName
        };
    }
}
