using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class MinMaxProperty
{
    public PropertyName Name { get; set; }
    public int MinBaseValue { get; set; }
    public int MaxBaseValue { get; set; }
    public int MinCurrentValue { get; set; }
    public int MaxCurrentValue { get; set; }
}
