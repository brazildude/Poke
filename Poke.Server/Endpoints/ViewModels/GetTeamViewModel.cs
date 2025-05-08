namespace Poke.Server.Endpoints.ViewModels;

public class GetTeamViewModel
{   
    public int TeamID { get; set; }
    public required string Name { get; set; }
    public List<string> Units { get; set; } = new List<string>();
}   