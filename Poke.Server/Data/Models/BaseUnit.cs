using System.ComponentModel.DataAnnotations.Schema;
using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public abstract class BaseUnit
{
    public int BaseUnitID { get; set; }
    public string Name { get; set; } = null!;
    public virtual int Life { get; set; }
    public virtual int Mana { get; set; }
    public virtual bool IsAbleToAttack { get; set; }
    public virtual IList<BaseSkill> Skills { get; set; } = new List<BaseSkill>();


    public virtual bool ApplySkillCost(BaseSkill baseSkill)
    {
        switch (baseSkill.SkillCost.ToProperty)
        {
            case ApplyToProperty.Life: Life -= baseSkill.SkillCost.MaxValue; break;
            case ApplyToProperty.Mana: Mana -= baseSkill.SkillCost.MaxValue; break;
            default: throw new ArgumentOutOfRangeException(nameof(baseSkill.SkillCost.ToProperty));
        }

        return true;
    }

    public virtual bool CheckSkillCost(BaseSkill baseSkill)
    {
        bool hasResources;
        switch (baseSkill.SkillCost.ToProperty)
        {
            case ApplyToProperty.Life: hasResources = Life >= baseSkill.SkillCost.MaxValue; break;
            case ApplyToProperty.Mana: hasResources = Mana >= baseSkill.SkillCost.MaxValue; break;
            default: throw new ArgumentOutOfRangeException(nameof(baseSkill.SkillCost.ToProperty));
        }

        return hasResources;
    }

    public virtual void Defend(ApplyToProperty toProperty, int value)
    {
        switch (toProperty)
        {
            case ApplyToProperty.Life: Life -= value; break;
            case ApplyToProperty.Mana: Mana -= value; break;
            default: throw new ArgumentOutOfRangeException(nameof(toProperty));
        }
    }

    public virtual void Heal(ApplyToProperty toProperty, int value)
    {
        switch (toProperty)
        {
            case ApplyToProperty.Life: Life += value; break;
            case ApplyToProperty.Mana: Mana += value; break;
            default: throw new ArgumentOutOfRangeException(nameof(toProperty));
        }
    }

    public virtual bool IsAlive()
    {
        if (Life <= 0)
        {
            return false;
        }

        return true;
    }

    public virtual void UseSkill(BaseSkill baseSkill, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits, HashSet<int> targetIDs, int randomSeed)
    {
        ApplySkillCost(baseSkill);

        baseSkill.random = new Random(randomSeed);
        baseSkill.Execute(this, ownUnits, enemyUnits, targetIDs);
    }
}
