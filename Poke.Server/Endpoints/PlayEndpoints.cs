using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.GameLogic;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class PlayEndpoints
{
    public static void RegisterPlayEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/plays")
        .RequireAuthorization()
        .RequireCors();

        endpoints.MapPost("", Play);
    }

    public static Results<Ok<PlayVM>, NotFound, BadRequest> Play(PlayVM playVM, ICurrentUser currentUser)
    {
        if (!MatchmakingContext.Matches.TryGetValue(playVM.MatchID, out var match))
        {
            return TypedResults.NotFound();
        }

        if (currentUser.UserID != match.State.CurrentUserID)
        {
            return TypedResults.BadRequest();
        }

        if (!match.State.GetCurrentTeam().TryGetValue(playVM.UnitID, out var unitInAction))
        {
            return TypedResults.BadRequest();
        }

        if (!unitInAction.Skills.TryGetValue(playVM.SkillName, out var skillInAction))
        {
            return TypedResults.BadRequest();
        }

        MatchLogic.HandlePlay(match, unitInAction, skillInAction, playVM.TargetIDs);        

        return TypedResults.Ok(playVM);
    }
}
