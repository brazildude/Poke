using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public class ApplyValue
{
    public int ApplyValueID { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public ApplyType Type { get; set; }
    public ApplyToProperty ToProperty { get; set; }

    public int Value(Random random)
    {
        return random.Next(MinValue, MaxValue + 1);
    }

    public static ApplyValue New(int minValue, int maxValue, ApplyType type, ApplyToProperty toProperty)
    {
        return new ApplyValue
        {
            MinValue = minValue,
            MaxValue = maxValue,
            Type = type,
            ToProperty = toProperty
        };
    }
}
