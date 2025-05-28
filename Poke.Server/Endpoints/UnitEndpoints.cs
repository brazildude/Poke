using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player;
using Poke.Server.Data.Base;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared.Mappers;
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

    public static Ok<IEnumerable<UnitVM>> GetUnits(ICurrentUser currentUser, PlayerContext db)
    {
        var units = BaseContext.GetUnits()
            .Select(x =>
                new UnitVM(
                    x.UnitID,
                    x.Name.ToString(),
                    VMMapper.SelectProperties(x.FlatProperties),
                    x.Skills.Select(s => new SkillVM(s.Name.ToString(), VMMapper.SelectProperties(s.FlatProperties), VMMapper.SelectBehaviors(s.Behaviors)))
                )
            );

        return TypedResults.Ok(units);
    }

    public static Results<Ok<UnitVM>, BadRequest> GetUnit(int unitID, ICurrentUser currentUser, PlayerContext db)
    {
        var unit = db.Units
            .Include(x => x.FlatProperties)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.MinMaxProperties)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Target)
            .Include(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Costs).ThenInclude(x => x.FlatProperty)
            .Where(x => x.Team.UserID == currentUser.UserID && x.UnitID == unitID)
            .Select(u => new UnitVM(
                u.UnitID,
                u.Name.ToString(),
                VMMapper.SelectProperties(u.FlatProperties),
                u.Skills.Select(s => new SkillVM(s.Name.ToString(), VMMapper.SelectProperties(s.FlatProperties), VMMapper.SelectBehaviors(s.Behaviors)))
            ))
            .SingleOrDefault();

        if (unit == null)
        {
            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(unit);
    }
}
