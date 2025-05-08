namespace Poke.Server.Data.Models;

public class User
{
    public int UserID { get; set; }
    public required string ExternalID { get; set; }
    public string? Name { get; set; }

    public List<Team> Teams { get; set; } = new List<Team>();
}
