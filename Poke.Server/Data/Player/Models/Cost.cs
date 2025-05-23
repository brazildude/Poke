using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Properties;

namespace Poke.Server.Data.Player.Models;

public class Cost
{
    public int CostID { get; set; }
    public int BehaviorID { get; set; }
    public CostType CostType { get; set; }
    /// <summary>
    /// Name of the property that will be targeted.
    /// </summary>
    public PropertyName PropertyName { get; set; }

    public FlatProperty FlatProperty { get; set; } = null!;
    public Behavior Behavior { get; set; } = null!;

    public static Cost New(int value, CostType costType, PropertyName propertyName)
    {
        return new Cost
        {
            FlatProperty = FlatProperty.New(PropertyName.SkillCost, value),
            CostType = costType,
            PropertyName = propertyName
        };
    }
}
