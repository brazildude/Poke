namespace Poke.Server.Data.Models;

public class User
{
    public required string UserID { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public List<Team> Teams { get; set; } = new List<Team>();
}
