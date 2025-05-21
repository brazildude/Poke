using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models;

public abstract class Behavior
{
    public int BehaviorID { get; set; }
    public int SkillID { get; set; }
    public BehaviorName BehaviorName { get; set; }
    public virtual BehaviorType BehaviorType { get; set; }
    /// <summary>
    /// Name of the property that will be targeted.
    /// </summary>
    public virtual PropertyName PropertyName { get; set; }

    public virtual MinMaxProperty MinMaxProperty { get; set; } = null!;
    public virtual Target Target { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;

    public virtual List<Cost> Costs { get; set; } = new List<Cost>();
    public virtual List<FlatProperty> Properties { get; set; } = new List<FlatProperty>();


    [NotMapped]
    public Random random = null!;

    public virtual void Execute(Unit unitInAction, List<Unit> ownUnits, List<Unit> enemyUnits, HashSet<int> targetIDs, Random random)
    {
        this.random = random;
        ApplyCost(unitInAction);

        var unitTargets = SelectTargets(unitInAction, ownUnits, enemyUnits, targetIDs);

        foreach (var unitTarget in unitTargets)
        {
            var property = unitTarget.Properties.Single(x => x.PropertyName == PropertyName);
            var skillValue = random.Next(MinMaxProperty.MinCurrentValue, MinMaxProperty.MaxCurrentValue + 1);

            if (BehaviorType == BehaviorType.Damage)
            {

            }

            property.CurrentValue += skillValue;

        }

        TickCooldown();
    }

    public virtual void ApplyCost(Unit unitInAction)
    {
        foreach (var cost in Costs)
        {
            var property = unitInAction.Properties.Single(x => x.PropertyName == cost.PropertyName);

            var valueToApply = cost.CostType switch
            {
                CostType.Flat => property.CurrentValue + cost.FlatProperty.CurrentValue,
                CostType.Porcentage => property.BaseValue * cost.FlatProperty.CurrentValue / 100,
                _ => throw new InvalidOperationException($"{nameof(cost.CostType)}")
            };

            property.CurrentValue = valueToApply;
        }
    }

    public virtual void TickCooldown()
    {
        var cooldown = Properties.Single(x => x.PropertyName == PropertyName.Cooldown);
        var currentCooldown = Properties.Single(x => x.PropertyName == PropertyName.Cooldown);

        if (currentCooldown.CurrentValue > 1)
        {
            currentCooldown.CurrentValue -= 1;
        }
        else
        {
            currentCooldown.CurrentValue = currentCooldown.BaseValue;
        }
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
}
