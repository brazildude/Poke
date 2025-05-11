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

    public virtual bool CheckSkillCost(BaseSkill baseSkill)
    {
        bool hasResources;
        switch (baseSkill.SkillCost.ToProperty)
        {
            case ApplyToProperty.Life: hasResources = Life >= baseSkill.SkillCost.Value(); break;
            case ApplyToProperty.Mana: hasResources = Mana >= baseSkill.SkillCost.Value(); break;
            default: throw new ArgumentOutOfRangeException(nameof(baseSkill.SkillCost.ToProperty));
        }

        return hasResources;
    }

    public virtual bool CheckSkillTargets(BaseSkill skill, HashSet<int> targetIds, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits)
    {
        var targetType = skill.Target.TargetType;
        var targetDirection = skill.Target.TargetDirection;

        // Always valid for Random and Self target types
        if (targetType == TargetType.Random || targetType == TargetType.Self)
            return true;

        // Cannot be valid if no targets are selected
        if (targetIds.Count == 0)
            return false;

        var ownUnitIds = ownUnits.Select(u => u.BaseUnitID).ToHashSet();
        var enemyUnitIds = enemyUnits.Select(u => u.BaseUnitID).ToHashSet();
        var allUnitIds = ownUnitIds.Union(enemyUnitIds);

        // Ensure all selected targets are valid units
        if (!targetIds.All(id => allUnitIds.Contains(id)))
            return false;

        // Check for Single target constraints
        if (targetType == TargetType.Single)
        {
            if (targetIds.Count != 1)
                return false;

            var targetId = targetIds.First();
            if ((targetDirection == TargetDirection.Team && enemyUnitIds.Contains(targetId)) ||
                (targetDirection == TargetDirection.Enemy && ownUnitIds.Contains(targetId)))
            {
                return false;
            }
        }

        // Check for Multiple target constraints
        if (targetType == TargetType.Multiple)
        {
            if (targetIds.Count != skill.Target.Quantity)
                return false;

            if ((targetDirection == TargetDirection.Team && targetIds.Any(enemyUnitIds.Contains)) ||
                (targetDirection == TargetDirection.Enemy && targetIds.Any(ownUnitIds.Contains)))
            {
                return false;
            }
        }

        return true;
    }

    public virtual bool ApplySkillCost(BaseSkill baseSkill)
    {
        switch (baseSkill.SkillCost.ToProperty)
        {
            case ApplyToProperty.Life: Life -= baseSkill.SkillCost.Value(); break;
            case ApplyToProperty.Mana: Mana -= baseSkill.SkillCost.Value(); break;
            default: throw new ArgumentOutOfRangeException(nameof(baseSkill.SkillCost.ToProperty));
        }

        return true;
    }

    public virtual void Defend(ApplyValue applyValue)
    {
        switch (applyValue.ToProperty)
        {
            case ApplyToProperty.Life: Life -= applyValue.Value(); break;
            case ApplyToProperty.Mana: Mana -= applyValue.Value(); break;
            default: throw new ArgumentOutOfRangeException(nameof(applyValue.ToProperty));
        }
    }

    public virtual void Heal(ApplyValue applyValue)
    {
        switch (applyValue.ToProperty)
        {
            case ApplyToProperty.Life: Life += applyValue.Value(); break;
            case ApplyToProperty.Mana: Mana += applyValue.Value(); break;
            default: throw new ArgumentOutOfRangeException(nameof(applyValue.ToProperty));
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

    public virtual void UseSkill(BaseSkill baseSkill, HashSet<int> targetIDs, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits, int randomSeed)
    {
        baseSkill.Execute(this, targetIDs, ownUnits, enemyUnits, randomSeed);
    }
}
