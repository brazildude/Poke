using Poke.Core.Models;

namespace Poke.Core;

public abstract class BaseUnit
{
    public int UnitID { get; set; }
    public virtual int Life { get; set; }
    public virtual int Mana { get; set; }
    protected virtual IList<BaseSkill> Skills { get; set; } = new List<BaseSkill>();


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

    public virtual void UseSkill(int skillID, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits)
    {
        var skill = Skills.Single(x => x.SkillID == skillID);

        skill.Execute(this, ownUnits, enemyUnits);
    }
}
