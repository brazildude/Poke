using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class FlatProperty
{
    public int FlatPropertyID { get; set; }
    public int? UnitID { get; set; }
    public int? BehaviorID { get; set; }
    public int? SkillID { get; set; }
    public int? CostID { get; set; }
    public PropertyName Name { get; set; }
    public int BaseValue { get; set; }
    public int CurrentValue { get; set; }

    public Unit? Unit { get; set; }
    public Behavior? Behavior { get; set; }
    public Skill? Skill { get; set; }
    public Cost? Cost { get; set; }

    public static FlatProperty New(PropertyName name, int value)
    {
        return new FlatProperty
        {
            Name = name,
            BaseValue = value,
            CurrentValue = value
        };
    }

    public static FlatProperty New(PropertyName name, int baseValue, int currentValue)
    {
        return new FlatProperty 
        {
            Name = name,
            BaseValue = baseValue,
            CurrentValue = currentValue
        };
    }
}
