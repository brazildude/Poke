using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Infrastructure.Matchmaking;

namespace Poke.Server.Endpoints;

public static class PlayEndpoints
{
    public record PlayDTO(int MatchID, int UnitID, int SkillID, HashSet<int> TargetIDs);

    public static void RegisterPlayEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/plays")
        .RequireAuthorization()
        .RequireCors("_myAllowSpecificOrigins");

        endpoints.MapGet("", GetPlay);
        endpoints.MapPost("", Play);
    }

    public static async Task<Results<Ok<User>, NotFound>> GetPlay(int userID, PokeContext db) 
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

    public static Results<Ok<PlayDTO>, BadRequest> Play(PlayDTO playDTO, ICurrentUser currentUser, PokeContext db) 
    {
        if (!MatchmakingState.Matches.TryGetValue(playDTO.MatchID, out var match))
        {
            return TypedResults.BadRequest();
        }

        match.Play(currentUser.UserID, playDTO.UnitID, playDTO.SkillID, playDTO.TargetIDs);

        return TypedResults.Ok(playDTO);
    }
}
