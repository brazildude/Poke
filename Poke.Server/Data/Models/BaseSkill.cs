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

    public virtual void Execute(BaseUnit unitInAction, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits)
    {
        ApplySkillCost(unitInAction);

        var unitTargets = SelectTargets(ownUnits, enemyUnits);

        switch (ApplyValue.Type)
        {
            case ApplyType.Damage: ApplyDamage(unitTargets); break;
            case ApplyType.Heal: ApplyHeal(unitTargets); break;
            default: throw new ArgumentOutOfRangeException(nameof(ApplyValue.Type));
        }
    }

    public virtual bool ApplySkillCost(BaseUnit unitInAction)
    {
        switch (SkillCost.ToProperty)
        {
            case ApplyToProperty.Life: unitInAction.Life -= SkillCost.Value(); break;
            case ApplyToProperty.Mana: unitInAction.Mana -= SkillCost.Value(); break;
            default: throw new ArgumentOutOfRangeException(nameof(SkillCost.ToProperty));
        }

        return true;
    }

    public virtual List<BaseUnit> SelectTargets(List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits)
    {
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
