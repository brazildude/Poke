using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class Cost
{
    public CostType CostType { get; set; }
    public PropertyName CostPropertyName { get; set; }
    public int BaseValue { get; set; }
    public int CurrentValue { get; set; }
}
