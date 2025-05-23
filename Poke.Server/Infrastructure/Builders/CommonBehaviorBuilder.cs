using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Behaviors;
using Poke.Server.Data.Player.Models.Properties;

namespace Poke.Server.Infrastructure.Builders;

public class CommonBehaviorBuilder
{
    private readonly CommonBehavior _behavior = new CommonBehavior();

    public static CommonBehaviorBuilder Create(BehaviorName name)
    {
        var builder = new CommonBehaviorBuilder();
        builder._behavior.BehaviorName = name;
        return builder;
    }

    public CommonBehaviorBuilder WithTarget(TargetType targetType, TargetDirection targetDirection, PropertyName targetPropertyName, int? quantity = 0)
    {
        _behavior.Target = Target.New(targetType, targetDirection, targetPropertyName, quantity);
        return this;
    }

    public CommonBehaviorBuilder WithMinMax(PropertyName property, int min, int max)
    {
        _behavior.MinMaxProperties = new List<MinMaxProperty> { MinMaxProperty.New(property, min, max) };
        return this;
    }

    public CommonBehaviorBuilder WithBehaviorType(BehaviorType behaviorType)
    {
        _behavior.BehaviorType = behaviorType;
        return this;
    }

    public CommonBehaviorBuilder WithCooldown(int cooldown)
    {
        _behavior.Properties ??= [];
        _behavior.Properties.Add(FlatProperty.New(PropertyName.Cooldown, cooldown));
        return this;
    }

    public CommonBehaviorBuilder WithCosts(List<Cost> costs)
    {
        _behavior.Costs = costs;
        return this;
    }

    public CommonBehavior Build()
    {
        return _behavior;
    }
}
