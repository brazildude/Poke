using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class Unit
{
    public int UnitID { get; set; }
    public UnitName Name { get; set; }

    public Dictionary<SkillName, Skill> Skills { get; set; } = [];
    public Dictionary<PropertyName, FlatProperty> FlatProperties { get; set; } = [];
}
