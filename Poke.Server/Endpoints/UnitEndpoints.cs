using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player;
using Poke.Server.Data.Base;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Properties;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class UnitEndpoints
{
    public static void RegisterUnitEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/units")
            .RequireAuthorization()
            .RequireCors();

        endpoints.MapGet("", GetUnits);
        endpoints.MapGet("{unitID}", GetUnit);
    }

    public static Results<Ok<UnitVM>, BadRequest> GetUnit(int unitID, ICurrentUser currentUser, PlayerContext db)
    {
        var unit = db.Units
            .Include(x => x.Properties)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.MinMaxProperty)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Target)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Costs).ThenInclude(x => x.FlatProperty)
            .Where(x => x.Team.UserID == currentUser.UserID && x.UnitID == unitID)
            .Select(u => new UnitVM(
                u.UnitID,
                u.UnitName.ToString(),
                SelectProperties(u.Properties),
                u.Skills.Select(s => new SkillVM(s.SkillName.ToString(), SelectProperties(s.Properties), SelectBehaviors(s.Behaviors)))
            ))
            .SingleOrDefault();

        if (unit == null)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(unit);
    }

    public static Ok<IEnumerable<UnitVM>> GetUnits(ICurrentUser currentUser, PlayerContext db)
    {
        var units = BaseContext.GetUnits()
            .Select(x =>
                new UnitVM(
                    x.UnitID,
                    x.UnitName.ToString(),
                    SelectProperties(x.Properties),
                    x.Skills.Select(s => new SkillVM(s.SkillName.ToString(), SelectProperties(s.Properties), SelectBehaviors(s.Behaviors)))
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
                b.Target.Quantity,
                SelectCosts(b.Costs)
            )
        );
    }
}
