using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class FlatProperty
{
    public PropertyName Name { get; set; }
    public int BaseValue { get; set; }
    public int CurrentValue { get; set; }


    /// <summary>
    /// CurrentValue reset it's value to BaseValue
    /// </summary>
    public void Reset()
    {
        CurrentValue = BaseValue;
    }
}
