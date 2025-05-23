using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class FlatProperty
{
    public PropertyName Name { get; set; }
    public int BaseValue { get; set; }
    public int CurrentValue { get; set; }
}
