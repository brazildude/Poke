using System.Runtime.InteropServices;
using Poke.Server.Data.Match.Models;
using Poke.Server.GameLogic.Events;
using Poke.Server.Shared.Enums;

namespace Poke.Server.GameLogic;

public class BehaviorLogic
{
    private static readonly Dictionary<BehaviorName, Func<Behavior, bool>> behaviorFuncs = [];

    static BehaviorLogic()
    {
        // add custom code if any
    }

    public static Action<MatchState, Unit, Behavior, HashSet<int>> Execute =
        (MatchState matchState, Unit unitInAction, Behavior behavior, HashSet<int> targetUnitIDs) =>
    {
        if (!TrySelectTargets(matchState, unitInAction, behavior, targetUnitIDs, out var unitTargets))
        {
            return;
        }

        if (!TryApplyCost(matchState, unitInAction, behavior))
        {
            return;
        }

        foreach (var unitTarget in unitTargets)
        {
            foreach (var minMaxProperty in behavior.MinMaxProperties)
            {
                var skillValue = matchState.RandomNextInt(minMaxProperty.MinCurrentValue, minMaxProperty.MaxCurrentValue + 1);

                var applyValue = behavior.Type switch
                {
                    BehaviorType.Damage => -skillValue,
                    BehaviorType.Heal => skillValue,
                    _ => throw new InvalidOperationException($"Unsupported behavior type: {behavior.Type}")
                };

                var e = unitTarget.ChangeFlatProperty("Hit", behavior.Target.TargetPropertyName, applyValue, HitType.Normal);
                matchState.AddEvent(e);
            }
        }
    };

    public static bool TryApplyCost(MatchState matchState, Unit unitInAction, Behavior behavior)
    {
        foreach (var cost in behavior.Costs)
        {
            var property = unitInAction.FlatProperties[cost.CostPropertyName];

            int valueToApply = cost.CostType switch
            {
                CostType.Flat => cost.CurrentValue,
                CostType.Percentage => (int)(property.BaseValue * cost.CurrentValue / 100f),
                _ => throw new InvalidOperationException($"{nameof(cost.CostType)}")
            };

            // Check if the cost can be paid
            if (property.CurrentValue < valueToApply)
            {
                matchState.AddEvent(new NoResourcesEvent
                {
                    Type = "NoResourcesEvent",
                    BehaviorName = behavior.Name.ToString(),
                    PropertyName = cost.CostPropertyName.ToString(),
                    RequiredValue = valueToApply,
                    CurrentValue = property.CurrentValue
                });
                
                return false;
            }
        }

        foreach (var cost in behavior.Costs)
        {
            var property = unitInAction.FlatProperties[cost.CostPropertyName];

            int valueToApply = cost.CostType switch
            {
                CostType.Flat => cost.CurrentValue,
                CostType.Percentage => (int)(property.BaseValue * cost.CurrentValue / 100f),
                _ => throw new InvalidOperationException($"{nameof(cost.CostType)}")
            };

            var e = unitInAction.ChangeFlatProperty("ApplyCost", cost.CostPropertyName, -valueToApply, HitType.Normal);
            matchState.AddEvent(e);
        }

        return true;
    }

    private static bool TrySelectTargets(
        MatchState matchState,
        Unit unitInAction,
        Behavior behavior,
        HashSet<int> targetIDs,
        out List<Unit> targets)
    {
        targets = null!;
        bool success;

        var targetType = behavior.Target.Type;
        var targetDirection = behavior.Target.Direction;

        if (targetType == TargetType.Self)
        {
            targets = [unitInAction];
            success = true;
        }
        else
        {
            var aliveOwn = matchState.GetCurrentTeam().Values.Where(UnitLogic.IsAlive).ToList();
            var aliveEnemy = matchState.GetEnemyTeam().Values.Where(UnitLogic.IsAlive).ToList();

            IEnumerable<Unit> GetUnits(TargetDirection dir) => dir switch
            {
                TargetDirection.Both => aliveOwn.Concat(aliveEnemy),
                TargetDirection.Own => aliveOwn,
                TargetDirection.Enemy => aliveEnemy,
                _ => throw new ArgumentOutOfRangeException(nameof(dir))
            };

            var candidates = GetUnits(targetDirection).ToList();
            if (candidates.Count == 0)
            {
                targets = [];
                success = false;
            }
            else
            {
                var quantity = behavior.Target.Quantity ?? 0;
                targets = targetType switch
                {
                    TargetType.All => candidates,
                    TargetType.Select when targetIDs.Count > 0 =>
                        candidates.Where(u => targetIDs.Contains(u.UnitID)).ToList(),
                    TargetType.Select => [],
                    TargetType.Random =>
                        SelectRandom(matchState, candidates, Math.Min(candidates.Count, quantity)),
                    _ => throw new InvalidOperationException("Unsupported target type.")
                };

                success = targets.Count > 0;
            }
        }

        return success;
    }

    private static List<Unit> SelectRandom(MatchState matchState, List<Unit> units, int quantity)
    {
        var span = CollectionsMarshal.AsSpan(units);
        matchState.RandomShuffle(span);

        var result = new List<Unit>(quantity);
        for (int i = 0; i < quantity; i++)
        {
            result.Add(span[i]);
        }

        return result;
    }
}
