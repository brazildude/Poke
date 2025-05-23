using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Units;

namespace Poke.Tests.Projects.Server.Data.Models.Skills;

public class FireballTests
{
    [Fact]
    public void Fireball01_ApplyDamageToAllEnemyTeam()
    {
        // arrange
        var unitInAction = new Mage { UnitID = 1 };
        var skill = unitInAction.Skills.Single(x => x.SkillName == SkillName.Fireball);
        var behavior = skill.Behaviors.Single(x => x.BehaviorName == BehaviorName.Fireball01);
        var ownUnits = new List<Unit> { unitInAction, new Paladin { UnitID = 2 }, new Warrior { UnitID = 3 }, new Rogue { UnitID = 4 } };
        var enemyUnits = new List<Unit> { new Mage { UnitID = 5 }, new Paladin { UnitID = 6 }, new Warrior { UnitID = 7 }, new Rogue { UnitID = 8 } };

        // act
        var unitTargets = behavior.SelectTargets(unitInAction, ownUnits, enemyUnits, null!);

        foreach (var unitTarget in unitTargets)
        {
            behavior.Execute(unitInAction, ownUnits, enemyUnits, null!, Random.Shared);
        }

        // assert
        foreach (var enemyUnit in enemyUnits)
        {
            var targetProperty = enemyUnit.Properties.Single(x => x.PropertyName == behavior.PropertyName);
            Assert.True(targetProperty.CurrentValue >= targetProperty.BaseValue + behavior.MinMaxProperty.MinCurrentValue);
            Assert.True(targetProperty.CurrentValue <= targetProperty.BaseValue + behavior.MinMaxProperty.MaxCurrentValue);
        }
    }
    
    [Fact]
    public void Fireball01_ApplCostToCurrentUnit()
    {
        // arrange
        var unitInAction = new Mage { UnitID = 1 };
        var skill = unitInAction.Skills.Single(x => x.SkillName == SkillName.Fireball);
        var behavior = skill.Behaviors.Single(x => x.BehaviorName == BehaviorName.Fireball01);
        var ownUnits = new List<Unit> { unitInAction, new Paladin { UnitID = 2 }, new Warrior { UnitID = 3 }, new Rogue{ UnitID  = 4} };
        var enemyUnits = new List<Unit> { new Mage { UnitID = 5 } , new Paladin { UnitID = 6 }, new Warrior { UnitID = 7 }, new Rogue { UnitID = 8} };

        // act
        var unitTargets = behavior.SelectTargets(unitInAction, ownUnits, enemyUnits, null!);
        behavior.Execute(unitInAction, ownUnits, enemyUnits, null!, Random.Shared);

        // assert
        foreach (var cost in behavior.Costs)
        {
            var unitCostPropertyTarget = unitInAction.Properties.Single(x => x.PropertyName == cost.PropertyName);
            Assert.True(unitCostPropertyTarget.CurrentValue == unitCostPropertyTarget.BaseValue + cost.FlatProperty.CurrentValue);
        }
    }
}
