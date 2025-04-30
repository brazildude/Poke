namespace Poke.Core.Models;

public class ApplyValue
{
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public ApplyType Type { get; set; }
    public ApplyToProperty ToProperty { get; set; }

    public int Value()
    {
        return Random.Shared.Next(MinValue, MaxValue + 1);
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
