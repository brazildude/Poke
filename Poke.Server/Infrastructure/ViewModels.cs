namespace Poke.Server.Infrastructure;

public static class ViewModels
{
    public record class PlayVM(Guid MatchID, int UnitID, int SkillID, HashSet<int> TargetIDs);
    public record BehaviorVM(string Type, string TargetProperty, int MinValue, int MaxValue, string TargetType, string TargetDirection, int? TargetQuantity, IEnumerable<CostVM> Costs);
    public record CostVM(string Type, string ToProperty, int Value);
    public record EditSkillVM(int SkillID, HashSet<int> BehaviorIDs);
    public record SkillVM(string Name, IEnumerable<FlatPropertyVM> Properties, IEnumerable<BehaviorVM> Behaviors);
    public record FlatPropertyVM(string Name, int Value);
    public record GetTeamVM(int TeamID, string Name, List<KeyValuePair<int, string>> Units);
    public record CreateTeamVM(string Name, HashSet<string> Units);
    public record EditTeamVM(int TeamID, string Name, HashSet<string> Units);
    public record UnitVM(int UnitID, string UnitName, IEnumerable<FlatPropertyVM> Properties, IEnumerable<SkillVM> Skills);
    public record UserVM(string ExternalID, string? Name, string? Email);
    public record CreateUserVM(string Provider, string Token);
}
