using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Properties;

public class MinMaxProperty
{
    public int MinMaxPropertyID { get; set; }
    public int MinBaseValue { get; set; }
    public int MaxBaseValue { get; set; }
    public int MinCurrentValue { get; set; }
    public int MaxCurrentValue { get; set; }
    public PropertyName PropertyName { get; set; }

    public static MinMaxProperty New(PropertyName propertyType, int minValue, int maxValue)
    {
        return new MinMaxProperty 
        {
            PropertyName = propertyType,
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
            PropertyName = propertyType,
            MinBaseValue = minBase,
            MaxBaseValue = maxBase,
            MinCurrentValue = minCurrent,
            MaxCurrentValue = maxCurrent
        };
    }
}
