using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
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

    public static Results<Ok<GetTeamVM>, NotFound> GetTeam(int teamID, ICurrentUser currentUser, PokeDbContext db)
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

    public static Results<Ok, BadRequest<string>> CreateTeam(CreateTeamVM viewModel, ICurrentUser currentUser, PokeDbContext db)
    {
        if (viewModel.Units.Count != 4)
        {
            return TypedResults.BadRequest("You must select 4 units.");
        }

        if (!viewModel.Units.All(x => PokeBaseContext.GetUnits().Select(p => p.UnitName.ToString()).Contains(x)))
        {
            return TypedResults.BadRequest("Invalid unit name.");
        }

        if (db.Teams.Any(x => x.UserID == currentUser.UserID && x.Name == viewModel.Name))
        {
            return TypedResults.BadRequest("Team name already exists.");
        }

        var team = new Team
        {
            UserID = currentUser.UserID,
            Name = viewModel.Name,
            Units = viewModel.Units.Select(PokeBaseContext.GetUnit).ToList()
        };

        db.Teams.Add(team);
        db.SaveChanges();

        return TypedResults.Ok();
    }

    public static Results<Ok, BadRequest<string>> EditTeam(EditTeamVM viewModel, ICurrentUser currentUser, PokeDbContext db)
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
        
        if (!viewModel.Units.All(x => PokeBaseContext.GetUnits().Select(p => p.UnitName.ToString()).Contains(x)))
        {
            return TypedResults.BadRequest("Invalid units name.");
        }
        
        team.Name = viewModel.Name;

        var unitsToBeAdded = PokeBaseContext.GetUnits()
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
