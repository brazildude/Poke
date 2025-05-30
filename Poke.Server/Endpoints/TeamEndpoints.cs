using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player;
using Poke.Server.Data.Base;
using Poke.Server.Data.Player.Models;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class TeamEndpoints
{
    public static void RegisterTeamEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/teams")
            .RequireAuthorization()
            .RequireCors();

        endpoints.MapGet("{teamID}", GetTeam);
        endpoints.MapPost("", CreateTeam);
        endpoints.MapPatch("", EditTeam);
    }

    public static Results<Ok<GetTeamVM>, NotFound> GetTeam(int teamID, ICurrentUser currentUser, PlayerContext playerContext)
    {
        var team = playerContext
            .Teams
            .Where(x => x.UserID == currentUser.UserID && x.TeamID == teamID)
            .Select(x =>
                new GetTeamVM(
                    x.TeamID,
                    x.Name,
                    x.Units.Select(u => new KeyValuePair<int, string>(u.UnitID, u.Name.ToString())).ToList())
                )
            .AsNoTracking()
            .SingleOrDefault();

        if (team == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(team);
    }

    public static Results<Ok, BadRequest<string>> CreateTeam(CreateTeamVM viewModel, ICurrentUser currentUser, PlayerContext playerContext)
    {
        if (viewModel.Units.Count != 4)
        {
            return TypedResults.BadRequest("You must select 4 units.");
        }

        var allUnitNames = BaseContext.GetUnits().Select(p => p.Name);
        if (!viewModel.Units.All(x => allUnitNames.Contains(x)))
        {
            return TypedResults.BadRequest("Invalid unit name.");
        }

        if (playerContext.Teams.Any(x => x.UserID == currentUser.UserID && x.Name == viewModel.Name))
        {
            return TypedResults.BadRequest("Team name already exists.");
        }

        var team = new Team
        {
            UserID = currentUser.UserID,
            Name = viewModel.Name,
            Units = viewModel.Units.Select(BaseContext.GetUnit).ToList()
        };

        playerContext.Teams.Add(team);
        playerContext.SaveChanges();

        return TypedResults.Ok();
    }

    public static Results<Ok, BadRequest<string>> EditTeam(EditTeamVM viewModel, ICurrentUser currentUser, PlayerContext playerContext)
    {
        if (viewModel.Units.Count != 4)
        {
            return TypedResults.BadRequest("You must select 4 units.");
        }

        // Load team with units and validate ownership
        var team = playerContext.Teams
            .Include(t => t.Units)
            .SingleOrDefault(t => t.UserID == currentUser.UserID && t.TeamID == viewModel.TeamID);

        if (team == null)
        {
            return TypedResults.BadRequest("Team does not exist.");
        }

        // Validate all provided unit names exist
        var allValidUnitNames = BaseContext.GetUnits().Select(u => u.Name).ToHashSet();
        if (!viewModel.Units.All(name => allValidUnitNames.Contains(name)))
        {
            return TypedResults.BadRequest("One or more unit names are invalid.");
        }

        // Update team name
        team.Name = viewModel.Name;

        // Determine current vs. desired unit names
        var currentUnitNames = team.Units.Select(u => u.Name).ToHashSet();
        var targetUnitNames = viewModel.Units;

        var toAddNames = targetUnitNames.Except(currentUnitNames).ToHashSet();
        var toRemoveNames = currentUnitNames.Except(targetUnitNames).ToHashSet();

        // Add new units
        if (toAddNames.Count > 0)
        {
            var unitsToAdd = BaseContext.GetUnits()
                .Where(u => toAddNames.Contains(u.Name))
                .ToList();

            team.Units.AddRange(unitsToAdd);
        }

        // Remove old units from DB
        if (toRemoveNames.Count > 0)
        {
            var unitsToRemove = team.Units
                .Where(u => toRemoveNames.Contains(u.Name))
                .ToList(); // Avoid modifying collection while iterating

            foreach (var unit in unitsToRemove)
            {
                team.Units.Remove(unit);
                playerContext.Units.Remove(unit); // Remove from DB explicitly
            }
        }

        // Save changes
        playerContext.SaveChanges();
        return TypedResults.Ok();
    }
}
