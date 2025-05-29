using Poke.Server.GameLogic.Events;
using Poke.Server.Shared.Enums;

namespace Poke.Server.Infrastructure;

public static class ViewModels
{
    public record UserVM(string ExternalID, string? Name, string? Email);
    public record CreateUserVM(string Provider, string Token);

    public record GetTeamVM(int TeamID, string Name, List<KeyValuePair<int, string>> Units);
    public record CreateTeamVM(string Name, HashSet<UnitName> Units);
    public record EditTeamVM(int TeamID, string Name, HashSet<string> Units);

    public record PlayVM(Guid MatchID, int UnitID, SkillName SkillName, HashSet<int> TargetIDs);
    public record PlayResultVM(List<GameEvent> Events, string? ErrorMessage); // Do I need a VM for this?

    public record UnitVM(int UnitID, string UnitName, IEnumerable<FlatPropertyVM> Properties, IEnumerable<SkillVM> Skills);

    public record EditSkillVM(int SkillID, HashSet<int> BehaviorIDs);
    public record SkillVM(string Name, IEnumerable<FlatPropertyVM> Properties, IEnumerable<BehaviorVM> Behaviors);
    
    public record BehaviorVM(string Type, string TargetProperty, string TargetType, string TargetDirection, int? TargetQuantity, IEnumerable<MinMaxPropertyVM> MinMaxProperties, IEnumerable<CostVM> Costs);
    public record CostVM(string Type, string ToProperty, int Value);
    public record FlatPropertyVM(string Name, int Value);
    public record MinMaxPropertyVM(string Name, int MinValue, int MaxValue);
}
