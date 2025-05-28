using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class MinMaxProperty
{
    public int MinMaxPropertyID { get; set; }
    public int? BehaviorID { get; set; }
    public int MinBaseValue { get; set; }
    public int MaxBaseValue { get; set; }
    public int MinCurrentValue { get; set; }
    public int MaxCurrentValue { get; set; }
    public PropertyName Name { get; set; }

    public Behavior? Behavior { get; set; }

    public static MinMaxProperty New(PropertyName name, int minValue, int maxValue)
    {
        return new MinMaxProperty
        {
            Name = name,
            MinBaseValue = minValue,
            MaxBaseValue = maxValue,
            MinCurrentValue = minValue,
            MaxCurrentValue = maxValue
        };
    }

    public static MinMaxProperty New(PropertyName propertyType, int minBase, int maxBase, int minCurrent, int maxCurrent)
    {
        return new MinMaxProperty 
        {
            Name = propertyType,
            MinBaseValue = minBase,
            MaxBaseValue = maxBase,
            MinCurrentValue = minCurrent,
            MaxCurrentValue = maxCurrent
        };
    }
}
