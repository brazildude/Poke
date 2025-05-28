using Poke.Server.Shared.Enums;
using Poke.Server.Data.Player.Models;

namespace Poke.Server.Infrastructure.Builders;

public class BehaviorBuilder
{
    private readonly Behavior behavior = new Behavior();

    public static BehaviorBuilder Create(BehaviorName name)
    {
        var builder = new BehaviorBuilder();
        builder.behavior.Name = name;
        return builder;
    }

    public BehaviorBuilder WithTarget(TargetType targetType, TargetDirection targetDirection, PropertyName targetPropertyName, int? quantity = 0)
    {
        behavior.Target = Target.New(targetType, targetDirection, targetPropertyName, quantity);
        return this;
    }

    public BehaviorBuilder WithMinMax(PropertyName property, int min, int max)
    {
        behavior.MinMaxProperties = new List<MinMaxProperty> { MinMaxProperty.New(property, min, max) };
        return this;
    }

    public BehaviorBuilder WithBehaviorType(BehaviorType behaviorType)
    {
        behavior.Type = behaviorType;
        return this;
    }

    public BehaviorBuilder WithCooldown(int cooldown)
    {
        behavior.FlatProperties ??= [];
        behavior.FlatProperties.Add(FlatProperty.New(PropertyName.Cooldown, cooldown));
        return this;
    }

    public BehaviorBuilder WithCosts(List<Cost> costs)
    {
        behavior.Costs = costs;
        return this;
    }

    public Behavior Build()
    {
        return behavior;
    }
}
