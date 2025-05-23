using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player;
using Poke.Server.Data.Base;
using Poke.Server.Data.Player.Models;
using Poke.Server.Data.Player.Models.Properties;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class SkillEndpoints
{
    public static void RegisterSkillEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/skills")
            .RequireAuthorization()
            .RequireCors();

        endpoints.MapGet("{unitName}", GetSkills);
    }

    public static Results<Ok<IEnumerable<SkillVM>>, BadRequest<string>> GetSkills(string unitName, ICurrentUser currentUser, PlayerContext db)
    {
        var unit = BaseContext.GetUnits().Where(x => x.UnitName.ToString() == unitName).SingleOrDefault();

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

    public static Results<Ok, BadRequest<string>> EditSkill(EditSkillVM viewModel, ICurrentUser currentUser, PlayerContext db)
    {
        var skill = db.Skills
            .Include(x => x.Unit)
            .ThenInclude(x => x.Team)
            .ThenInclude(x => x.User)
            .SingleOrDefault(x => x.SkillID == viewModel.SkillID && x.Unit.Team.UserID == currentUser.UserID);

        if (skill == null)
        {
            return TypedResults.BadRequest("Invalid skill.");
        }

        return TypedResults.Ok();
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
