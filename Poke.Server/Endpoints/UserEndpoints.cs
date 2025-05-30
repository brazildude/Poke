using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player;
using Poke.Server.Data.Player.Models;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/users")
            .RequireAuthorization()
            .RequireCors();

        endpoints.MapPost("", CreateUser).AllowAnonymous();
        endpoints.MapGet("{userID}", GetUser);
        endpoints.MapPost("teams", GetTeams);
    }

    public static async Task<Results<Ok, BadRequest, UnauthorizedHttpResult>> CreateUser(CreateUserVM viewModel, IAuthService authService, PlayerContext playerContext)
    {
        if (viewModel == null)
        {
            return TypedResults.BadRequest();
        }

        if (viewModel.Provider == null || viewModel.Token == null)
        {
            return TypedResults.BadRequest();
        }

        var uuid = await authService.VerifyIdTokenAsync(viewModel.Token);

        if (uuid == null)
        {
            return TypedResults.Unauthorized();
        }

        if (!await playerContext.Users.AnyAsync(x => x.UserID == uuid))
        {
            var user = new User
            {
                UserID = uuid
            };

            await playerContext.Users.AddAsync(user);
            await playerContext.SaveChangesAsync();
        }

        return TypedResults.Ok();
    }

    public static Ok<UserVM> GetUser(ICurrentUser currentUser, PlayerContext playerContext)
    {
        var user = playerContext.Users
            .Where(x => x.UserID == currentUser.UserID)
            .Select(x => new UserVM(x.UserID, x.Name, x.Email))
            .AsNoTracking()
            .Single();

        return TypedResults.Ok(user);
    }

    public static Ok<List<GetTeamVM>> GetTeams(ICurrentUser currentUser, PlayerContext playerContext)
    {
        var teams = playerContext.Teams
            .Include(x => x.Units)
            .Where(x => x.UserID == currentUser.UserID)
            .Select(x => new GetTeamVM(
                     x.TeamID,
                     x.Name,
                     x.Units.Select(u => new KeyValuePair<int, string>(u.UnitID, u.Name.ToString())).ToList())
                 )
            .AsNoTracking()
            .ToList();

        return TypedResults.Ok(teams);
    }
}
