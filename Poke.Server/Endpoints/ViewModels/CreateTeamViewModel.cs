namespace Poke.Server.Endpoints.ViewModels;

public class CreateTeamViewModel
{   
    public required string Name { get; set; }
    public HashSet<int> UnitIDs { get; set; } = new HashSet<int>();
}
