namespace Poke.Server.Data.Player.Models;

public class Team
{
    public int TeamID { get; set; }
    public string UserID { get; set; } = null!;
    public required string Name { get; set; }

    public User User { get; set; } = null!;
    public List<Unit> Units { get; set; } = new List<Unit>();
}
