using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models.Properties;

namespace Poke.Server.Data.Player.Models;

public abstract class Unit
{
    public int UnitID { get; set; }
    public int TeamID { get; set; }
    public UnitName UnitName { get; set; }

    public Team Team { get; set; } = null!;
    public virtual List<Skill> Skills { get; set; } = new List<Skill>();
    public virtual List<FlatProperty> Properties { get; set; } = new List<FlatProperty>();

    public virtual bool CheckSkillCost(Skill skill)
    {
        var hasResource = true;

        foreach (var cost in skill.Behaviors.SelectMany(x => x.Costs))
        {
            var property = Properties.Single(x => x.PropertyName == cost.PropertyName);

            hasResource = cost.CostType switch
            {
                CostType.Flat => property.CurrentValue > Math.Abs(cost.FlatProperty.CurrentValue),
                CostType.Percentage => property.CurrentValue > property.BaseValue * cost.FlatProperty.CurrentValue / 100,
                _ => throw new InvalidOperationException($"{nameof(cost.CostType)}")
            };

            if (hasResource == false)
            {
                break;
            }
        }

        return hasResource;
    }

    public virtual bool IsAlive()
    {
        var isAlive = Properties.Single(x => x.PropertyName == PropertyName.Life).CurrentValue > 0;

        return isAlive;
    }

    public virtual bool CanPlay()
    {
        var playTimes = Properties.Single(x => x.PropertyName == PropertyName.PlayTimes);

        return playTimes.CurrentValue >= 1;
    }

    public virtual void UseSkill(Skill skill, List<Unit> ownUnits, List<Unit> enemyUnits, HashSet<int> targetIDs, int randomSeed)
    {
        skill.random = new Random(randomSeed);
        skill.Execute(this, ownUnits, enemyUnits, targetIDs);

        var playTimes = Properties.Single(x => x.PropertyName == PropertyName.PlayTimes);
        playTimes.CurrentValue -= 1;
    }
}
