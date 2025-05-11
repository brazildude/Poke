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

    public virtual void Execute(BaseUnit unitInAction, HashSet<int> targetIDs, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits, int randomSeed)
    {
        var unitTargets = SelectTargets(unitInAction, targetIDs, ownUnits, enemyUnits);

        switch (ApplyValue.Type)
        {
            case ApplyType.Damage: ApplyDamage(unitTargets); break;
            case ApplyType.Heal: ApplyHeal(unitTargets); break;
            default: throw new ArgumentOutOfRangeException(nameof(ApplyValue.Type));
        }
    }

    public virtual List<BaseUnit> SelectTargets(BaseUnit unitInAction, HashSet<int> targetIDs, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits)
    {
        if (Target.TargetType == TargetType.Self)
        {
            return new List<BaseUnit> { unitInAction };
        }

        return enemyUnits;
    }

    public virtual void ApplyDamage(List<BaseUnit> unitTargets)
    {
        foreach (var unitTarget in unitTargets)
        {
            unitTarget.Defend(ApplyValue);
        }
    }

    public virtual void ApplyHeal(List<BaseUnit> unitTargets)
    {
        foreach (var unitTarget in unitTargets)
        {
            unitTarget.Heal(ApplyValue);
        }
    }
}
