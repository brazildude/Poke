using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models;

public class Behavior
{
    public int BehaviorID { get; set; }
    public int MinMaxPropertyID { get; set; }
    public ApplyType Type { get; set; }
    public PropertyName TargetProperty { get; set; }
    public MinMaxProperty MinMaxProperty { get; set; } = null!;
    public Target Target { get; set; } = null!;

    [NotMapped]
    public Random random = null!;

    public virtual void Execute(Unit unitTarget, Random random)
    {
        var property = unitTarget.Properties.Single(x => x.PropertyName == TargetProperty);
        var skillValue = random.Next(MinMaxProperty.MinCurrentValue, MinMaxProperty.MaxCurrentValue + 1);

        if (Type == ApplyType.Damage)
        {

        }

        property.CurrentValue += skillValue;
    }

    public virtual List<Unit> SelectTargets(
        Unit unitInAction,
        List<Unit> ownUnits,
        List<Unit> enemyUnits,
        HashSet<int> targetIDs)
    {
        var targetType = Target.TargetType;
        var targetDirection = Target.TargetDirection;

        // Self-targeting case
        if (targetType == TargetType.Self)
            return [unitInAction];

        var aliveOwnUnits = ownUnits.Where(u => u.IsAlive());
        var aliveEnemyUnits = enemyUnits.Where(u => u.IsAlive());

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
        var targetQuantity = Math.Min(selectableUnits.Count, Target.Quantity ?? 0);

        return targetType switch
        {
            TargetType.Random => SelectRandom(selectableUnits, targetQuantity),
            TargetType.Select => selectableUnits
                .Where(unit => targetIDs.Contains(unit.UnitID))
                .ToList(),
            TargetType.All => selectableUnits,
            _ => throw new InvalidOperationException("Unsupported target type.")
        };
    }

    private List<Unit> SelectRandom(List<Unit> units, int quantity)
    {
        var span = CollectionsMarshal.AsSpan(units);

        random.Shuffle(span);
        var shuffledUnits = new List<Unit>(quantity);
        foreach (var unit in span.Slice(0, quantity))
        {
            shuffledUnits.Add(unit);
        }

        return shuffledUnits;
    }

    public static Behavior New(int minValue, int maxValue, ApplyType applyType, PropertyName toPropertyName, Target target)
    {
        return new Behavior
        {
            Target = target,
            MinMaxProperty = MinMaxProperty.New(PropertyName.ApplyValue, minValue, maxValue),
            Type = applyType,
            TargetProperty = toPropertyName
        };
    }
}
