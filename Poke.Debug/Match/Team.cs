using Poke.Core;

namespace Poke.Debug.Match;

public class Team
{
    public int TeamID { get; set; }
    public int UserID { get; set; }

    public List<BaseUnit> Units { get; set; } = new List<BaseUnit>();
}
