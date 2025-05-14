using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public abstract class Unit
{
    public int UnitID { get; set; }
    public int BaseUnitID { get; set; }
    public int TeamID { get; set; }
    public string Name { get; set; } = null!;
    public virtual int Life { get; set; }
    public virtual int Mana { get; set; }
    public virtual bool IsAbleToAttack { get; set; }

    public Team Team { get; set; } = null!;
    public virtual IList<Skill> Skills { get; set; } = new List<Skill>();


    public virtual bool ApplySkillCost(Skill skill)
    {
        switch (skill.Cost.ToProperty)
        {
            case ApplyToProperty.Life: Life -= skill.Cost.Value; break;
            case ApplyToProperty.Mana: Mana -= skill.Cost.Value; break;
            default: throw new ArgumentOutOfRangeException(nameof(skill.Cost.ToProperty));
        }

        return true;
    }

    public virtual bool CheckSkillCost(Skill skill)
    {
        bool hasResources;
        switch (skill.Cost.ToProperty)
        {
            case ApplyToProperty.Life: hasResources = Life >= skill.Cost.Value; break;
            case ApplyToProperty.Mana: hasResources = Mana >= skill.Cost.Value; break;
            default: throw new ArgumentOutOfRangeException(nameof(skill.Cost.ToProperty));
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

    public virtual void UseSkill(Skill skill, List<Unit> ownUnits, List<Unit> enemyUnits, HashSet<int> targetIDs, int randomSeed)
    {
        ApplySkillCost(skill);

        skill.random = new Random(randomSeed);
        skill.Execute(this, ownUnits, enemyUnits, targetIDs);
    }
}
