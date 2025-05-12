namespace Poke.Server.Data.Models;

public class Team
{
    public int TeamID { get; set; }
    public int UserID { get; set; }
    public required string Name { get; set; }

    public User User { get; set; } = null!;
    public List<Unit> Units { get; set; } = new List<Unit>();
}
