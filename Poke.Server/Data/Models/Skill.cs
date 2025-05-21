using System.ComponentModel.DataAnnotations.Schema;
using Poke.Server.Data.Enums;
using Poke.Server.Data.Models.Properties;

namespace Poke.Server.Data.Models;

public abstract class Skill
{
    public int SkillID { get; set; }
    public int UnitID { get; set; }
    public SkillName SkillName { get; set; }

    public virtual List<Behavior> Behaviors { get; set; } = new List<Behavior>();
    public virtual List<FlatProperty> Properties { get; set; } = new List<FlatProperty>();
    
    public Unit Unit { get; set; } = null!;

    [NotMapped]
    public Random random = null!;

    public virtual bool IsInCooldown()
    {
        var isInCooldown = Behaviors.SelectMany(x => x.Properties).Any(x => x.PropertyName == PropertyName.Cooldown && x.CurrentValue > 0);
        
        return isInCooldown;
    }

    public virtual void Execute(Unit unitInAction, List<Unit> ownUnits, List<Unit> enemyUnits, HashSet<int> targetIDs)
    {
        foreach (var behavior in Behaviors)
        {
            behavior.Execute(unitInAction, ownUnits, enemyUnits, targetIDs, random);
        }
    }
}
