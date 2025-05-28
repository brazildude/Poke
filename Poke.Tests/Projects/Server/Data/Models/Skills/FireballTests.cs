using Poke.Server.Shared.Enums;
using Poke.Server.GameLogic;
using Poke.Tests.Infrastructure;

namespace Poke.Tests.Projects.Server.Data.Models.Skills;

public class FireballTests
{
    [Fact]
    public void Fireball01_ApplyDamageToAllEnemyTeam()
    {
        // arrange
        var match = MatchGenerator.CreateMatch();
        var unitInAction = match.State.Teams.First().Value.First(x => x.Value.Name == UnitName.Mage).Value;
        var skillInAction = unitInAction.Skills[SkillName.Fireball];
        var behavior = skillInAction.Behaviors.Single(x => x.Name == BehaviorName.Fireball01);

        // act
        BehaviorLogic.Execute(match.State, unitInAction, behavior, new HashSet<int> { 5, 6, 7, 8 });

        // assert
        foreach (var enemyUnit in match.State.Teams.Last().Value.Values)
        {
            var targetProperty = enemyUnit.FlatProperties[behavior.Target.PropertyName];
            Assert.True(targetProperty.CurrentValue >= targetProperty.BaseValue - behavior.MinMaxProperties.First().MaxCurrentValue);
            Assert.True(targetProperty.CurrentValue <= targetProperty.BaseValue - behavior.MinMaxProperties.First().MinCurrentValue);
        }
    }

    [Fact]
    public void Fireball01_ApplCostToCurrentUnit()
    {
        // arrange
        var match = MatchGenerator.CreateMatch();
        var unitInAction = match.State.Teams.First().Value.First(x => x.Value.Name == UnitName.Mage).Value;
        var skillInAction = unitInAction.Skills[SkillName.Fireball];
        var behavior = skillInAction.Behaviors.Single(x => x.Name == BehaviorName.Fireball01);

        // act
        BehaviorLogic.Execute(match.State, unitInAction, behavior, new HashSet<int> { 5, 6, 7, 8 });

        // assert
        foreach (var cost in behavior.Costs)
        {
            var unitCostPropertyTarget = unitInAction.FlatProperties[cost.CostPropertyName];
            Assert.True(unitCostPropertyTarget.CurrentValue == unitCostPropertyTarget.BaseValue + cost.CurrentValue);
        }
    }
}
