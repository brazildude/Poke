using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Infrastructure;
using Poke.Server.Infrastructure.Auth;

namespace Poke.Server.Endpoints;

public static class SkillEndpoints
{
    public record BehaviorVM(string Type, string TargetProperty, int MinValue, int MaxValue, string TargetType, string TargetDirection, int? TargetQuantity, IEnumerable<CostVM> Costs);
    public record CostVM(string Type, string ToProperty, int Value);
    public record SkillVM(string Name, IEnumerable<FlatPropertyVM> Properties, IEnumerable<BehaviorVM> Behaviors);
    public record FlatPropertyVM(string Name, int Value);

    public static void RegisterSkillEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/skills")
            .RequireAuthorization()
            .RequireCors();

        endpoints.MapGet("{unitName}", GetSkills);
    }

    public static Results<Ok<IEnumerable<SkillVM>>, BadRequest<string>> GetSkills(string unitName, ICurrentUser currentUser, PokeContext db)
    {
        var unit = Game.GetUnits().Where(x => x.UnitName.ToString() == unitName).SingleOrDefault();

        if (unit == null)
        {
            return TypedResults.BadRequest("Invalid unit name.");
        }

        var skills =
            unit.Skills.Select(x =>
                new SkillVM(
                    x.SkillName.ToString(),
                    SelectProperties(x.Properties),
                    SelectBehaviors(x.Behaviors)
                )
            );

        return TypedResults.Ok(skills);
    }
    
    private static IEnumerable<FlatPropertyVM> SelectProperties(List<FlatProperty> x)
    {
        return x.Select(p => new FlatPropertyVM(p.PropertyName.ToString(), p.CurrentValue));
    }

    private static IEnumerable<CostVM> SelectCosts(List<Cost> x)
    {
        return x.Select(c => new CostVM(c.CostType.ToString(), c.PropertyName.ToString(), c.FlatProperty.CurrentValue));
    }

    private static IEnumerable<BehaviorVM> SelectBehaviors(List<Behavior> x)
    {
        return x.Select(b =>
            new BehaviorVM(
                b.BehaviorType.ToString(),
                b.PropertyName.ToString(),
                b.MinMaxProperty.MinCurrentValue,
                b.MinMaxProperty.MaxCurrentValue,
                b.Target.TargetType.ToString(),
                b.Target.TargetDirection.ToString(),
                b.Target.Quantity,
                SelectCosts(b.Costs)
            )
        );
    }
}
