using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models;

public class Unit
{
    public int UnitID { get; set; }
    public int TeamID { get; set; }
    public UnitName Name { get; set; }

    public Team Team { get; set; } = null!;
    public virtual List<Skill> Skills { get; set; } = [];
    public virtual List<FlatProperty> FlatProperties { get; set; } = [];
}
