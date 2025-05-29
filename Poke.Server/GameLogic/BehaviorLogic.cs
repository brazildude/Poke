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

        var random = new Random(matchState.RandomSeed);

        foreach (var unitTarget in unitTargets)
        {
            var property = unitTarget.FlatProperties[behavior.Target.TargetPropertyName];

            foreach (var minMaxProperty in behavior.MinMaxProperties)
            {
                var skillValue = random.Next(minMaxProperty.MinCurrentValue, minMaxProperty.MaxCurrentValue + 1);

                switch (behavior.Type)
                {
                    case BehaviorType.Damage:
                        property.CurrentValue -= skillValue;
                        break;

                    case BehaviorType.Heal:
                        property.CurrentValue += skillValue;
                        break;

                    default:
                        throw new InvalidOperationException($"Unsupported behavior type: {behavior.Type}");
                }
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

            property.CurrentValue += valueToApply;

            matchState.AddEvent(new CostEvent
            {
                Type = "Cost",
                UnitID = unitInAction.UnitID,
                CostPropertyName = cost.CostPropertyName.ToString(),
                CostValue = cost.CurrentValue,
            });
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

        var targetType = behavior.Target.Type;
        var targetDirection = behavior.Target.Direction;

        // Self-targeting
        if (targetType == TargetType.Self)
        {
            targets = [unitInAction];
            return true;
        }

        // Gather alive units
        var aliveOwn = matchState.GetCurrentTeam()
            .Values.Where(UnitLogic.IsAlive);
        var aliveEnemy = matchState.GetEnemyTeam()
            .Values.Where(UnitLogic.IsAlive);

        IEnumerable<Unit> GetUnits(TargetDirection dir) => dir switch
        {
            TargetDirection.Both => aliveOwn.Concat(aliveEnemy),
            TargetDirection.Own => aliveOwn,
            TargetDirection.Enemy => aliveEnemy,
            _ => throw new ArgumentOutOfRangeException(nameof(dir))
        };

        var candidates = GetUnits(targetDirection).ToList();

        // Early exit if no candidates
        if (candidates.Count == 0)
        {
            targets = [];
            return false;
        }

        var quantity = behavior.Target.Quantity ?? 0;

        targets = targetType switch
        {
            TargetType.All => candidates,
            TargetType.Select => candidates
                .Where(u => targetIDs.Contains(u.UnitID))
                .ToList(),
            TargetType.Random => SelectRandom(matchState, candidates, Math.Min(candidates.Count, quantity)),
            _ => throw new InvalidOperationException("Unsupported target type.")
        };

        return targets.Count > 0;
    }


    private static List<Unit> SelectRandom(MatchState matchState, List<Unit> units, int quantity)
    {
        var span = CollectionsMarshal.AsSpan(units);

        new Random(matchState.RandomSeed).Shuffle(span);
        var shuffledUnits = new List<Unit>(quantity);
        foreach (var unit in span.Slice(0, quantity))
        {
            shuffledUnits.Add(unit);
        }

        return shuffledUnits;
    }
}
