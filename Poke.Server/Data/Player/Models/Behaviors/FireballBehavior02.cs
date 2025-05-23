using Poke.Server.Data.Player.Models.Properties;
using Poke.Server.Shared.Enums;

namespace Poke.Server.Data.Player.Models.Behaviors;

public class FireballBehavior02 : Behavior
{
    public FireballBehavior02()
    {
        Target = new Target { TargetType = TargetType.All, TargetDirection = TargetDirection.Enemy, Quantity = 1 };
        MinMaxProperty = MinMaxProperty.New(PropertyName.BehaviorValue, -20, -15);
        PropertyName = PropertyName.Life;
        Properties.Add(FlatProperty.New(PropertyName.BehaviorValue, 1));
        Costs.Add(Cost.New(-5, CostType.Percentage, PropertyName.Life));
    }

    public override void Execute(Unit unitInAction, List<Unit> ownUnits, List<Unit> enemyUnits, HashSet<int> targetIDs, Random random)
    {
        this.random = random;
        ApplyCost(unitInAction);

        var unitTargets = SelectTargets(unitInAction, ownUnits, enemyUnits, targetIDs);

        for (var i = 0; i < 2; i++)
        {
            foreach (var unitTarget in unitTargets)
            {
                var property = unitTarget.Properties.Single(x => x.PropertyName == PropertyName);
                var skillValue = random.Next(MinMaxProperty.MinCurrentValue, MinMaxProperty.MaxCurrentValue + 1);

                if (BehaviorType == BehaviorType.Damage)
                {

                }

                property.CurrentValue += skillValue;

            }
        }
    }
}
