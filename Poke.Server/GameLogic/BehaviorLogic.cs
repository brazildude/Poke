using System.Runtime.InteropServices;
using Poke.Server.Data.Match.Models;
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
        var unitTargets = SelectTargets(matchState, unitInAction, behavior, targetUnitIDs);
        var random = new Random(matchState.RandomSeed);
        
        foreach (var unitTarget in unitTargets)
        {
            var property = unitTarget.FlatProperties[behavior.Target.PropertyName];

            foreach (var minMaxProperty in behavior.MinMaxProperties)
            {
                var skillValue = random.Next(minMaxProperty.MinCurrentValue, minMaxProperty.MaxCurrentValue + 1);

                if (behavior.Type == BehaviorType.Damage)
                {

                }

                property.CurrentValue += skillValue;
            }
        }
    };

    public static Action<Unit, Behavior> ApplyCost = (unitInAction, behavior) =>
    {
        foreach (var cost in behavior.Costs)
        {
            var property = unitInAction.FlatProperties[cost.CostPropertyName];

            var valueToApply = cost.CostType switch
            {
                CostType.Flat => cost.CurrentValue,
                CostType.Percentage => property.BaseValue * cost.CurrentValue / 100,
                _ => throw new InvalidOperationException($"{nameof(cost.CostType)}")
            };

            property.CurrentValue += valueToApply;
        }
    };

    private static List<Unit> SelectTargets(MatchState matchState, Unit unitInAction, Behavior behavior, HashSet<int>targetIDs)
    {
        var targetType = behavior.Target.Type;
        var targetDirection = behavior.Target.Direction;

        // Self-targeting case
        if (targetType == TargetType.Self)
            return [unitInAction];

        var aliveOwnUnits = matchState.GetCurrentTeam().Where(u => UnitLogic.IsAlive(u.Value)).Select(x => x.Value);
        var aliveEnemyUnits = matchState.GetEnemyTeam().Where(u => UnitLogic.IsAlive(u.Value)).Select(x => x.Value);

        IEnumerable<Unit> GetUnits(TargetDirection direction)
        {
            return direction switch
            {
                TargetDirection.Both => aliveOwnUnits.Concat(aliveEnemyUnits),
                TargetDirection.Own => aliveOwnUnits,
                TargetDirection.Enemy => aliveEnemyUnits,
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
        }

        var selectableUnits = GetUnits(targetDirection).ToList();
        var targetQuantity = Math.Min(selectableUnits.Count, behavior.Target.Quantity ?? 0);

        return targetType switch
        {
            TargetType.Random => SelectRandom(matchState, selectableUnits, targetQuantity),
            TargetType.Select => selectableUnits
                .Where(unit => targetIDs.Contains(unit.UnitID))
                .ToList(),
            TargetType.All => selectableUnits,
            _ => throw new InvalidOperationException("Unsupported target type.")
        };
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
