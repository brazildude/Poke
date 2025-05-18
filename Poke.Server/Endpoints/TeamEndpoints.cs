using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Infrastructure;
using Poke.Server.Infrastructure.Auth;

namespace Poke.Server.Endpoints;

public static class TeamEndpoints
{
    public record GetTeamVM(int TeamID, string Name, List<KeyValuePair<int, string>> Units);
    public record CreateTeamVM(string Name, HashSet<string> Units);
    public record EditTeamVM(int TeamID, string Name, HashSet<string> Units);

    public static void RegisterTeamEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/teams")
            .RequireAuthorization()
            .RequireCors("myAllowSpecificOrigins");

        endpoints.MapGet("{teamID}", GetTeam);
        endpoints.MapPost("", CreateTeam);
        endpoints.MapPatch("", EditTeam);
    }

    public static Results<Ok<GetTeamVM>, NotFound> GetTeam(int teamID, ICurrentUser currentUser, PokeContext db)
    {
        var team = db
            .Teams
            .Where(x => x.UserID == currentUser.UserID && x.TeamID == teamID)
            .Select(x =>
                new GetTeamVM(
                    x.TeamID,
                    x.Name,
                    x.Units.Select(u => new KeyValuePair<int, string>(u.UnitID, u.UnitName.ToString())).ToList())
                )
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
            .Select(x => new GetTeamVM(
                    x.TeamID,
                    x.Name,
                    x.Units.Select(u => new KeyValuePair<int, string>(u.UnitID, u.UnitName.ToString())).ToList())
                )
            .ToList();

        return TypedResults.Ok(teams);
    }

    public static Results<Ok, BadRequest<string>> CreateTeam(CreateTeamVM viewModel, ICurrentUser currentUser, PokeContext db)
    {
        if (viewModel.Units.Count != 4)
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
            Units = viewModel.Units.Select(Game.GetUnit).ToList()
        };

        db.Teams.Add(team);
        db.SaveChanges();

        return TypedResults.Ok();
    }

    public static Results<Ok, BadRequest<string>> EditTeam(EditTeamVM viewModel, ICurrentUser currentUser, PokeContext db)
    {
        if (viewModel.Units.Count != 4)
        {
            return TypedResults.BadRequest("You must select 4 units.");
        }

        var team = db.Teams
            .Include(x => x.Units)
            .Where(x => x.UserID == currentUser.UserID && x.TeamID == viewModel.TeamID)
            .SingleOrDefault();

        if (team == null)
        {
            return TypedResults.BadRequest("Team does not exist.");
        }
        
        if (!viewModel.Units.All(x => Game.GetUnits().Select(p => p.UnitName.ToString()).Contains(x)))
        {
            return TypedResults.BadRequest("Invalid units name.");
        }
        
        team.Name = viewModel.Name;

        var unitsToBeAdded = Game.GetUnits()
            .Where(x =>
                viewModel.Units
                .Except(team.Units.Select(x => x.UnitName.ToString()))
                .Contains(x.UnitName.ToString()))
            .ToList();

        team.Units.AddRange(unitsToBeAdded);

        db.Units.Where(x => x.TeamID == viewModel.TeamID && !viewModel.Units.Contains(x.UnitName.ToString())).ExecuteDelete();
        db.SaveChanges();

        return TypedResults.Ok();
    }
}
