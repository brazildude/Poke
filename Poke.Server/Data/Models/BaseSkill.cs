using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;
using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public abstract class BaseSkill
{
    public int BaseSkillID { get; set; }
    public int SkillCostID { get; set; }
    public int ApplyValueID { get; set; }
    public int TargetID { get; set; }
    public virtual ApplyValue SkillCost { get; set; } = null!;
    public virtual ApplyValue ApplyValue { get; set; } = null!;
    public virtual Target Target { get; set; } = null!;
    public virtual int TotalCooldown { get; set; }
    public virtual int CurrentCooldown { get; set; }

    [NotMapped]
    public Random random = null!;

    public virtual bool IsInCooldown()
    {
        return CurrentCooldown == 0;
    }

    public virtual void Execute(BaseUnit unitInAction, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits, HashSet<int> targetIDs)
    {
        var unitTargets = SelectTargets(unitInAction, ownUnits, enemyUnits, targetIDs);

        switch (ApplyValue.Type)
        {
            case ApplyType.Damage: ApplyDamage(unitTargets); break;
            case ApplyType.Heal: ApplyHeal(unitTargets); break;
            default: throw new ArgumentOutOfRangeException(nameof(ApplyValue.Type));
        }
    }

    public virtual List<BaseUnit> SelectTargets(
        BaseUnit unitInAction,
        List<BaseUnit> ownUnits,
        List<BaseUnit> enemyUnits,
        HashSet<int> targetIDs)
    {
        var targetType = Target.TargetType;
        var targetDirection = Target.TargetDirection;

        // Self-targeting case
        if (targetType == TargetType.Self)
            return [unitInAction];

        var aliveOwnUnits = ownUnits.Where(u => u.IsAlive());
        var aliveEnemyUnits = enemyUnits.Where(u => u.IsAlive());

        IEnumerable<BaseUnit> GetUnits(TargetDirection direction)
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
        var targetQuantity = Math.Min(selectableUnits.Count, Target.Quantity);

        return targetType switch
        {
            TargetType.Random => SelectRandom(selectableUnits, targetQuantity),
            TargetType.Select => selectableUnits
                .Where(unit => targetIDs.Contains(unit.BaseUnitID))
                .ToList(),
            _ => throw new InvalidOperationException("Unsupported target type.")
        };
    }

    public virtual void ApplyDamage(List<BaseUnit> unitTargets)
    {
        foreach (var unitTarget in unitTargets)
        {
            unitTarget.Defend(ApplyValue.ToProperty, ApplyValue.Value(random));
        }
    }

    public virtual void ApplyHeal(List<BaseUnit> unitTargets)
    {
        foreach (var unitTarget in unitTargets)
        {
            unitTarget.Heal(ApplyValue.ToProperty, ApplyValue.Value(random));
        }
    }

    private List<BaseUnit> SelectRandom(List<BaseUnit> units, int quantity)
    {
        var span = CollectionsMarshal.AsSpan(units);

        random.Shuffle(span);
        var baseUnits = new List<BaseUnit>(quantity);
        foreach (var baseUnit in span.Slice(0, quantity))
        {
            baseUnits.Add(baseUnit);
        }

        return baseUnits;
    }
}
