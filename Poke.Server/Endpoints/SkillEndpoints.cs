using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player;
using Poke.Server.Data.Base;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared.Mappers;
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
        var unit = BaseContext.GetUnits().Where(x => x.Name.ToString() == unitName).SingleOrDefault();

        if (unit == null)
        {
            return TypedResults.BadRequest("Invalid unit name.");
        }

        var skills =
            unit.Skills.Select(x =>
                new SkillVM(
                    x.Name.ToString(),
                    VMMapper.SelectProperties(x.FlatProperties),
                    VMMapper.SelectBehaviors(x.Behaviors)
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
}
