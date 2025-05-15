using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Infrastructure;
using Poke.Server.Infrastructure.Auth;

namespace Poke.Server.Endpoints;

public static class TeamEndpoints
{
    public record GetTeamVM(int TeamID, string Name, Dictionary<int, string> Units);
    public record CreateTeamVM(string Name, HashSet<int> BaseUnitIDs);

    public static void RegisterTeamEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/teams")
            .RequireAuthorization()
            .RequireCors("myAllowSpecificOrigins");

        endpoints.MapGet("{teamID}", GetTeam);
        endpoints.MapPost("", CreateTeam);
    }

    public static Results<Ok<GetTeamVM>, NotFound> GetTeam(int teamID, ICurrentUser currentUser, PokeContext db)
    {
        var team = db
            .Teams
            .Where(x => x.UserID == currentUser.UserID && x.TeamID == teamID)
            .Select(x => new GetTeamVM(x.TeamID, x.Name, x.Units.ToDictionary(u => u.UnitID, u => u.UnitName.ToString())))
            .SingleOrDefault();

        if (team == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(team);
    }

    public static Ok<List<GetTeamVM>> GetTeams(ICurrentUser currentUser, PokeContext db)
    {
        var teams = db.Teams
            .Include(x => x.Units)
            .Where(x => x.UserID == currentUser.UserID)
            .Select(t => new GetTeamVM(t.TeamID, t.Name, t.Units.ToDictionary(u => u.UnitID, u => u.UnitName.ToString())))
            .ToList();

        return TypedResults.Ok(teams);
    }

    public static Results<Ok, BadRequest<string>> CreateTeam(CreateTeamVM viewModel, ICurrentUser currentUser, PokeContext db)
    {
        if (viewModel.BaseUnitIDs.Count != 4)
        {
            return TypedResults.BadRequest("You must select 4 units.");
        }

        if (db.Teams.Any(x => x.UserID == currentUser.UserID && x.Name == viewModel.Name))
        {
            return TypedResults.BadRequest("Team name already exists.");
        }

        var team = new Team
        {
            UserID = currentUser.UserID,
            Name = viewModel.Name,
            Units = viewModel.BaseUnitIDs.Select(Game.GetUnit).ToList()
        };

        db.Teams.Add(team);
        db.SaveChanges();

        return TypedResults.Ok();
    }
}
