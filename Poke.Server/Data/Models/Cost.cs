using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models;

public class Cost
{
    public int CostID { get; set; }
    public int SkillID { get; set; }
    public CostType CostType { get; set; }
    public PropertyName PropertyName { get; set; }
    public FlatProperty FlatProperty { get; set; } = null!;
    public Skill Skill { get; set; } = null!;

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
