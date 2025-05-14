using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models;

public abstract class Unit
{
    public int UnitID { get; set; }
    public int BaseUnitID { get; set; }
    public int TeamID { get; set; }
    public string Name { get; set; } = null!;

    public Team Team { get; set; } = null!;
    public virtual List<Skill> Skills { get; set; } = new List<Skill>();
    public virtual List<FlatProperty> Properties { get; set; } = new List<FlatProperty>();

    public virtual void ApplySkillCost(Skill skill)
    {
        var property = Properties.Single(x => x.PropertyName == skill.Cost.PropertyName);
        property.CurrentValue += skill.Cost.Value.CurrentValue;
    }
    
    public virtual bool CheckSkillCost(Skill skill)
    {
        var property = Properties.Single(x => x.PropertyName == skill.Cost.PropertyName);
        var hasResource = property.CurrentValue > skill.Cost.Value.CurrentValue;

        return hasResource;
    }

    public virtual void Defend(PropertyName toProperty, int value)
    {
        var property = Properties.Single(x => x.PropertyName == toProperty);
        property.CurrentValue += value;
    }

    public virtual void Heal(PropertyName toProperty, int value)
    {
        var property = Properties.Single(x => x.PropertyName == toProperty);
        property.CurrentValue += value;
    }

    public virtual bool IsAlive()
    {
        if (Properties.Single(x => x.PropertyName == PropertyName.Life).CurrentValue <= 0)
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
