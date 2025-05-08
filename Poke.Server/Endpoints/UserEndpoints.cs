using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Endpoints.ViewModels;

namespace Poke.Server.Endpoints;

public static class UserEndpoints
{
    public record class CreateUserViewModel(string Provider, string Token);

    public static void RegisterUserEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("api/users");

        userEndpoints.MapGet("{userID}", GetUser);
        userEndpoints.MapPost("", CreateUser);
        userEndpoints.MapPost("/teams", GetTeams);
        userEndpoints.MapGet("test", Test).RequireAuthorization();
    }

    public static async Task<Results<Ok<User>, NotFound>> GetUser(int userID, PokeContext db)
    {
        var user = await db.Users
        .Include(x => x.Teams).ThenInclude(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.SkillCost)
        .Include(x => x.Teams).ThenInclude(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.ApplyValue)
        .SingleOrDefaultAsync(x => x.UserID == userID);

        if (user == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(user);
    }

    public static async Task<Results<Ok, BadRequest<string>, UnauthorizedHttpResult>> CreateUser(CreateUserViewModel viewModel, PokeContext db)
    {
        if (viewModel == null)
        {
            return TypedResults.BadRequest("");
        }

        if (viewModel.Provider == null || viewModel.Token == null)
        {
            return TypedResults.BadRequest("");
        }

        var payload = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(viewModel.Token);

        if (payload == null)
        {
            return TypedResults.Unauthorized();
        }

        var user = new User
        {
            ExternalID = payload.Uid
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return TypedResults.Ok();
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
