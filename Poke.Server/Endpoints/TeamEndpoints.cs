using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Endpoints.ViewModels;
using Poke.Server.Infrastructure.Auth;

namespace Poke.Server.Endpoints;

public static class TeamEndpoints
{
    public static void RegisterTeamEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("api/teams");

        userEndpoints.MapGet("{teamID}", GetTeam).RequireAuthorization();
        userEndpoints.MapPost("", CreateTeam);
    }

    public static async Task<Results<Ok<Team>, NotFound>> GetTeam(int teamID, ICurrentUser currentUser, PokeContext db) 
    {
        var team = await db
        .Teams
        .SingleOrDefaultAsync(x => x.UserID == currentUser.UserID && x.TeamID == teamID);

        if (team == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(team);
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> CreateTeam(CreateTeamViewModel viewModel, ICurrentUser currentUser, PokeContext db) 
    {
        if (viewModel.UnitIDs.Count != 4)
        {
            return TypedResults.BadRequest("You must select 4 units.");
        }

        if (db.Teams.Any(x => x.UserID == currentUser.UserID && x.Name == viewModel.Name))
        {
            return TypedResults.BadRequest("Team name already exists.");
        }

        var team = new Team 
        { 
            Name = viewModel.Name
        };


        
        //db.Users.Add(user);
        //await db.SaveChangesAsync();

        return TypedResults.Ok("");
    }

    public static async Task<Results<Ok<List<GetTeamViewModel>>, NotFound>> GetTeams(PokeContext db) 
    {
        var userID = 1;

        var teams = await db.Teams
        .Include(x => x.Units)
        .Where(x => x.UserID == userID)
        .Select(t => new GetTeamViewModel
        {
            TeamID = t.TeamID,
            Name = t.Name,
            Units = t.Units.Select(p => p.Name).ToList()
        })
        .ToListAsync();

        return TypedResults.Ok(teams);
    }

    public static string Test()
    {
        return "OK";
    }
}
