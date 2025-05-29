using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.GameLogic;
using Poke.Server.GameLogic.Events;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Infrastructure.GameLogic;
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

    public static Results<Ok<List<GameEvent>>, BadRequest<string>, NotFound> Play(PlayVM playVM, ICurrentUser currentUser, MatchContext matchContext)
    {
        // TODO: this code need a lock on matchState
        if (CacheContext.Matches.TryGetValue(playVM.MatchID, out var matchState))
        {
            if (currentUser.UserID != matchState.CurrentUserID)
            {
                return TypedResults.BadRequest("It's not your turn.");
            }

            if (!matchState.GetCurrentTeam().TryGetValue(playVM.UnitID, out var unitInAction))
            {
                return TypedResults.BadRequest("Unit not found in your team.");
            }

            if (!unitInAction.Skills.TryGetValue(playVM.SkillName, out var skillInAction))
            {
                return TypedResults.BadRequest("Skill not found in unit.");
            }

            return MatchLogic.HandlePlay(matchState, unitInAction, skillInAction, playVM.TargetIDs)
                .Match<Results<Ok<List<GameEvent>>, BadRequest<string>, NotFound>>(
                _continue =>
                {
                    var turnEvents = matchState.GetTurnEvents();
                    return TypedResults.Ok(turnEvents);
                },
                matchFinished =>
                {
                    var match = matchContext.Matches.Single(x => x.MatchID == playVM.MatchID);
                    match.IsMatchOver = true;
                    match.UserWinnerID = matchFinished.WinnerPlayerID;
                    match.State = matchState;

                    var turnEvents = matchState.GetTurnEvents();

                    matchContext.Matches.Update(match);
                    matchContext.SaveChanges();

                    CacheContext.Matches.TryRemove(playVM.MatchID, out _);
                    return TypedResults.Ok(turnEvents);
                },
                error => TypedResults.BadRequest(error.Message)
            );
        }

        return TypedResults.NotFound();
    }
}
