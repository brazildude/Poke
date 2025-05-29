using Poke.Server.Data.Match.Models;
using Poke.Server.GameLogic.Events;
using Poke.Server.Shared.Enums;

namespace Poke.Server.GameLogic;

public class UnitLogic
{
    private static readonly Dictionary<UnitName, Func<Unit, bool>> isAliveFuncs = [];
    private static readonly Dictionary<UnitName, Func<Unit, bool>> canPlayFuncs = [];
    private static readonly Dictionary<UnitName, Func<Unit, Skill, bool>> hasInitialSkillResourcesFuncs = [];

    static UnitLogic()
    {
        // add custom code if any
    }

    public static Func<Unit, bool> IsAlive { get; set; } = (unit) =>
    {
        if (isAliveFuncs.TryGetValue(unit.Name, out var custom))
        {
            return custom(unit);
        }

        return unit.FlatProperties[PropertyName.Life].CurrentValue > 0;
    };

    public static Func<Unit, bool> CanPlay { get; set; } = (unit) =>
    {
        if (canPlayFuncs.TryGetValue(unit.Name, out var custom))
        {
            return custom(unit);
        }

        return unit.FlatProperties[PropertyName.PlayTimes].CurrentValue > 0;
    };

    public static Func<Unit, Skill, bool> HasInitialSkillResources { get; set; } = (unit, skill) =>
    {
        if (hasInitialSkillResourcesFuncs.TryGetValue(unit.Name, out var custom))
        {
            return custom(unit, skill);
        }

        var hasResources = true;
        var behavior = skill.Behaviors.First();

        foreach (var cost in behavior.Costs)
        {
            if (unit.FlatProperties[cost.CostPropertyName].CurrentValue < cost.CurrentValue)
            {
                hasResources = false;
                break;
            }
        }

        return hasResources;
    };

    public static Action<MatchState, Unit, Skill, HashSet<int>> UseSkill =
        (MatchState matchState, Unit unitInAction, Skill skillInAction, HashSet<int> targetIDs) =>
    {
        matchState.AddEvent(new UnitSelectedEvent { Type = nameof(UnitSelectedEvent), UnitID = unitInAction.UnitID  });
        matchState.AddEvent(new SkillSelectedEvent { Type = nameof(SkillSelectedEvent), SkillID = skillInAction.SkillID  });

        foreach (var behavior in skillInAction.Behaviors)
        {
            BehaviorLogic.Execute(matchState, unitInAction, behavior, targetIDs);
        }

        unitInAction.FlatProperties[PropertyName.PlayTimes].CurrentValue -= 1;
    };
}
