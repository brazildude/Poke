using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models.Properties;

public class FlatProperty
{
    public int FlatPropertyID { get; set; }
    public int? UnitID { get; set; }
    public int? BehaviorID { get; set; }
    public int? SkillID { get; set; }
    public int? CostID { get; set; }
    public int BaseValue { get; set; }
    public int CurrentValue { get; set; }
    public PropertyName PropertyName { get; set; }

    public Unit? Unit { get; set; }
    public Behavior? Behavior { get; set; }
    public Skill? Skill { get; set; }
    public Cost? Cost { get; set; }

    public void SetCurrentToBase()
    {
        CurrentValue = BaseValue;
    }

    public static FlatProperty New(PropertyName propertyType, int value)
    {
        return new FlatProperty
        {
            PropertyName = propertyType,
            BaseValue = value,
            CurrentValue = value
        };
    }

    public static FlatProperty New(PropertyName propertyType, int baseValue, int currentValue)
    {
        return new FlatProperty 
        {
            PropertyName = propertyType,
            BaseValue = baseValue,
            CurrentValue = currentValue
        };
    }
}
