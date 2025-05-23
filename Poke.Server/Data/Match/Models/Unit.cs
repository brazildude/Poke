using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Match.Models;

public class Unit
{
    public int UnitID { get; set; }
    public UnitName Name { get; set; }

    public List<Skill> Skills { get; set; } = [];
    public List<FlatProperty> FlatProperties { get; set; } = [];
}
