using MemoryPack;
using Poke.Server.GameLogic.Events;
using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

[MemoryPackable]
public partial class Unit
{
    public int UnitID { get; set; }
    public UnitName Name { get; set; }

    public Dictionary<SkillName, Skill> Skills { get; set; } = [];
    public Dictionary<PropertyName, FlatProperty> FlatProperties { get; set; } = [];

    public GameEvent ChangeFlatProperty(string eventType, PropertyName propertyName, int applyValue, HitType hitType)
    {
        var property = FlatProperties[propertyName];
        property.CurrentValue += applyValue;

        var e = new UnitStateChangedEvent
        {
            Type = eventType,
            UnitID = UnitID,
            PropertyName = propertyName.ToString(),
            AppliedValue = applyValue,
            CurrentValue = property.CurrentValue,
            HitType = hitType
        };

        return e;
    }
}
