using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public abstract class BaseUnit
{
    public int BaseUnitID { get; set; }
    public string Name { get; set; } = null!;
    public virtual int Life { get; set; }
    public virtual int Mana { get; set; }
    public virtual IList<BaseSkill> Skills { get; set; } = new List<BaseSkill>();

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
        var skill = Skills.Single(x => x.BaseSkillID == skillID);

        skill.Execute(this, ownUnits, enemyUnits);
    }
}
