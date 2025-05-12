using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Infrastructure.Auth;

namespace Poke.Server.Endpoints;

public static class UserEndpoints
{
    public record UserVM(int UserID, string? Name, string? Email);
    public record CreateUserVM(string Provider, string Token);
    public record GetTeamVM(int TeamID , string Name, List<string> Units);

    public static void RegisterUserEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/users")
            .RequireCors("_myAllowSpecificOrigins");

        endpoints.MapGet("{userID}", GetUser).RequireAuthorization();
        endpoints.MapPost("", CreateUser);
        endpoints.MapPost("/teams", GetTeams).RequireAuthorization();
    }

    public static Results<Ok<UserVM>, NotFound> GetUser(ICurrentUser currentUser, PokeContext db)
    {
        var user = db.Users
            .Select(x => new UserVM(x.UserID, x.Name, x.Email))
            .Where(x => x.UserID == currentUser.UserID)
            .Single();
        
        return TypedResults.Ok(user);
    }

    public static async Task<Results<Ok, BadRequest<string>, UnauthorizedHttpResult>> CreateUser(CreateUserVM viewModel, IAuthService authService, PokeContext db)
    {
        if (viewModel == null)
        {
            return TypedResults.BadRequest("");
        }

        if (viewModel.Provider == null || viewModel.Token == null)
        {
            return TypedResults.BadRequest("");
        }

        var uuid = await authService.VerifyIdTokenAsync(viewModel.Token);

        if (uuid == null)
        {
            return TypedResults.Unauthorized();
        }

        if (await db.Users.AnyAsync(x => x.ExternalID == uuid))
        {
            return TypedResults.Ok();
        }

        var user = new User
        {
            ExternalID = uuid
        };

        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }

    public static Results<Ok<List<GetTeamVM>>, NotFound> GetTeams(PokeContext db)
    {
        var userID = 1;

        var teams = db.Teams
        .Include(x => x.Units)
        .Where(x => x.UserID == userID)
        .Select(t => new GetTeamVM(t.TeamID, t.Name, t.Units.Select(p => p.Name).ToList()))
        .ToList();

        return TypedResults.Ok(teams);
    }
}
