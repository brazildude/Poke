using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models.Properties;

public class FlatProperty
{
    public int FlatPropertyID { get; set; }
    public int? UnitID { get; set; }
    public int? SkillID { get; set; }
    public int BaseValue { get; set; }
    public int CurrentValue { get; set; }
    public PropertyName PropertyName { get; set; }

    public Unit? Unit { get; set; }
    public Skill? Skill { get; set; }

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
