using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public class Cost
{
    public int CostID { get; set; }
    public int Value { get; set; }
    public CostType Type { get; set; }
    public ApplyToProperty ToProperty { get; set; }

    public static Cost New(int value, CostType type, ApplyToProperty toProperty)
    {
        return new Cost
        {
            Value = value,
            Type = type,
            ToProperty = toProperty
        };
    }
}
