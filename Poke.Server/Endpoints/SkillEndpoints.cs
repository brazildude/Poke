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
        endpoints.MapPost("edit", EditSkill);
    }

    public static Results<Ok<IEnumerable<SkillVM>>, BadRequest<string>> GetSkills(string unitName, ICurrentUser currentUser)
    {
        var unit = BaseContext.GetUnits().Where(x => x.Name.ToString() == unitName).SingleOrDefault();

        if (unit == null)
        {
            return TypedResults.BadRequest("Invalid unit name.");
        }

        var skills = VMMapper.SelectSkills(unit.Skills);

        return TypedResults.Ok(skills);
    }

    public static Results<Ok, BadRequest<string>> EditSkill(EditSkillVM viewModel, ICurrentUser currentUser, PlayerContext playerContext)
    {
        var skill = playerContext.Skills
            .Include(s => s.Behaviors)
            .Include(s => s.Unit)
                .ThenInclude(u => u.Team)
                    .ThenInclude(t => t.User)
            .Where(s =>
                s.Unit.TeamID == viewModel.TeamID &&
                s.UnitID == viewModel.UnitID &&
                s.SkillID == viewModel.SkillID &&
                s.Unit.Team.UserID == currentUser.UserID
            )
            .SingleOrDefault();

        if (skill == null)
        {
            return TypedResults.BadRequest("Invalid skill.");
        }

        var baseSkill = BaseContext.GetSkill(skill.Name);
        var allValidBehaviorIDs = baseSkill.Behaviors.Select(b => b.Name).ToHashSet();
        if (!viewModel.BehaviorIDs.All(id => allValidBehaviorIDs.Contains(id)))
        {
            return TypedResults.BadRequest("One or more behavior IDs are invalid.");
        }

        var currentBehaviorIDs = skill.Behaviors.Select(b => b.Name).ToHashSet();
        var targetBehaviorIDs = viewModel.BehaviorIDs;

        var toAdd = targetBehaviorIDs.Except(currentBehaviorIDs);
        var toRemove = currentBehaviorIDs.Except(targetBehaviorIDs);

        if (toAdd.Any())
        {
            var newBehaviors = baseSkill.Behaviors
                .Where(b => toAdd.Contains(b.Name))
                .ToList();

            skill.Behaviors.AddRange(newBehaviors);
        }

        if (toRemove.Any())
        {
            var toRemoveList = skill.Behaviors
                .Where(b => toRemove.Contains(b.Name))
                .ToList();

            foreach (var b in toRemoveList)
            {
                skill.Behaviors.Remove(b);
            }
        }

        playerContext.SaveChanges();

        return TypedResults.Ok();
    }
}
