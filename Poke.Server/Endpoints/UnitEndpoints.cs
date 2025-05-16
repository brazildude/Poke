using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Enums;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Properties;
using Poke.Server.Infrastructure;
using Poke.Server.Infrastructure.Auth;

namespace Poke.Server.Endpoints;

public static class UnitEndpoints
{
    public record BehaviorVM(string Type, string TargetProperty, int MinValue, int MaxValue, string TargetType, string TargetDirection, int? TargetQuantity);
    public record CostVM(string Type, string ToProperty, int Value);
    public record SkillVM(string Name, IEnumerable<FlatPropertyVM> Properties, IEnumerable<CostVM> Costs, IEnumerable<BehaviorVM> Behaviors);
    public record FlatPropertyVM(string Name, int Value);
    public record UnitVM(int UnitID, string UnitName, IEnumerable<FlatPropertyVM> Properties, IEnumerable<SkillVM> Skills);

    public static void RegisterUnitEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/units")
            .RequireAuthorization()
            .RequireCors("myAllowSpecificOrigins");

        endpoints.MapGet("", GetUnits);
        endpoints.MapGet("{unitID}", GetUnit);
    }

    public static Results<Ok<UnitVM>, BadRequest> GetUnit(int unitID, ICurrentUser currentUser, PokeContext db)
    {
        var unit = db.Units
            .Include(x => x.Properties)
            .Include(x => x.Skills).ThenInclude(x => x.Costs).ThenInclude(x => x.FlatProperty)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.MinMaxProperty)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Target)
            .Where(x => x.Team.UserID == currentUser.UserID && x.UnitID == unitID)
            .Select(u => new UnitVM(
                u.UnitID,
                u.UnitName.ToString(),
                SelectProperties(u.Properties),
                u.Skills.Select(s => new SkillVM(s.SkillName.ToString(), SelectProperties(s.Properties), SelectCosts(s.Costs), SelectBehaviors(s.Behaviors)))
            ))
            .SingleOrDefault();

        if (unit == null)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(unit);
    }

    public static Ok<IEnumerable<UnitVM>> GetUnits(ICurrentUser currentUser, PokeContext db)
    {
        var units = Game.GetUnits()
            .Select(x =>
                new UnitVM(
                    x.UnitID,
                    x.UnitName.ToString(),
                    SelectProperties(x.Properties),
                    x.Skills.Select(s => new SkillVM(s.SkillName.ToString(), SelectProperties(s.Properties), SelectCosts(s.Costs), SelectBehaviors(s.Behaviors)))
                )
            );

        return TypedResults.Ok(units);
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
                b.Target.Quantity
            )
        );
    }
}
