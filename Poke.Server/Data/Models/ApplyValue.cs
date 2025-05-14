using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models;

public class ApplyValue
{
    public int ApplyValueID { get; set; }
    public int MinMaxPropertyID { get; set; }
    public ApplyType Type { get; set; }
    public PropertyName PropertyName { get; set; }
    public MinMaxProperty MinMaxProperty { get; set; } = null!;

    public int GetValue(Random random)
    {
        return random.Next(MinMaxProperty.MinCurrentValue, MinMaxProperty.MaxCurrentValue + 1);
    }

    public static ApplyValue New(int minValue, int maxValue, ApplyType applyType, PropertyName propertyName)
    {
        return new ApplyValue
        {
            MinMaxProperty = MinMaxProperty.New(PropertyName.ApplyValue, minValue, maxValue),
            Type = applyType,
            PropertyName = propertyName
        };
    }
}
