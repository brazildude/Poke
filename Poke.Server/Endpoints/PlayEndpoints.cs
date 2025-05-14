using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Infrastructure.Matchmaking;

namespace Poke.Server.Endpoints;

public static class PlayEndpoints
{
    public record class PlayVM(int MatchID, int UnitID, int SkillID, HashSet<int> TargetIDs);

    public static void RegisterPlayEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/plays")
        .RequireAuthorization()
        .RequireCors("myAllowSpecificOrigins");

        endpoints.MapGet("", GetPlay);
        endpoints.MapPost("", Play);
    }

    public static async Task<Results<Ok<User>, NotFound>> GetPlay(ICurrentUser currentUser, PokeContext db) 
    {
        var user = await db.Users
        .Include(x => x.Teams).ThenInclude(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Cost)
        .Include(x => x.Teams).ThenInclude(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.ApplyValue)
        .SingleOrDefaultAsync(x => x.UserID == currentUser.UserID);

        if (user == null)
        {
            return TypedResults.NotFound();
        }
            
        return TypedResults.Ok(user);
    }

    public static Results<Ok<PlayVM>, BadRequest> Play(PlayVM playVM, ICurrentUser currentUser, PokeContext db) 
    {
        if (!MatchmakingState.Matches.TryGetValue(playVM.MatchID, out var match))
        {
            return TypedResults.BadRequest();
        }

        if (currentUser.UserID != match.CurrentUserID)
        {
            return TypedResults.BadRequest();
        }

        var unitInAction = match.GetCurrentTeam(currentUser.UserID).Units.SingleOrDefault(x => x.UnitID == playVM.UnitID);

        if (unitInAction == null)
        {
            return TypedResults.BadRequest();
        }

        var skill = unitInAction.Skills.SingleOrDefault(x => x.SkillID == playVM.SkillID);

        if (skill == null)
        {
            return TypedResults.BadRequest();
        }

        match.Play(unitInAction, skill, playVM.TargetIDs);

        return TypedResults.Ok(playVM);
    }
}
