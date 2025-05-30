using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.Data.Player;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class MatchmakingEndpoints
{
    public static void RegisterMatchmakingEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/matchmaking")
        .RequireAuthorization()
        .RequireCors();

        endpoints.MapGet("join", Join);
        endpoints.MapGet("cancel", Cancel);
        endpoints.MapGet("wait", Wait);
    }

    public static Results<Ok<string>, BadRequest<string>> Join(int teamID, ICurrentUser currentUser, PlayerContext playerContext, MatchContext matchContext)
    {
        if (!playerContext.Teams.Any(x => x.TeamID == teamID && x.UserID == currentUser.UserID))
        {
            return TypedResults.BadRequest("Team does not exist.");
        }

        var tcs = new TaskCompletionSource<(Guid, string)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var player = new MatchmakingContext.WaitingPlayer(currentUser.UserID, teamID, tcs);

        // Atomically try to register the player
        if (!MatchmakingContext.Waiters.TryAdd(currentUser.UserID, tcs))
        {
            return TypedResults.BadRequest("Already in queue.");
        }

        // Try to match
        if (!MatchmakingContext.Queue.TryDequeue(out var opponent))
        {
            // No match found, enqueue the current player
            MatchmakingContext.Queue.Enqueue(player);
            return TypedResults.Ok("Waiting for match...");
        }

        var createMatchVM = new CreateMatchVM(
            player.UserID, opponent.UserID,
            player.TeamID, opponent.TeamID);

        // In the future this will be a http call instead of a direct call
        var response = MatchEndpoints.CreateMatch(createMatchVM, matchContext, playerContext);

        if (response.Result is BadRequest<string> result)
        {
            MatchmakingContext.Waiters.TryRemove(currentUser.UserID, out _);
            MatchmakingContext.Waiters.TryRemove(opponent.UserID, out _);

            return TypedResults.BadRequest(result.Value);
        }

        var matchID = ((Ok<Guid>)response.Result).Value;

        opponent.Tcs.TrySetResult((matchID, "player1"));
        tcs.TrySetResult((matchID, "player2"));

        return TypedResults.Ok("Match found!");
    }

    public static Results<Ok<string>, Ok> Cancel(ICurrentUser currentUser)
    {
        MatchmakingContext.Waiters.TryRemove(currentUser.UserID, out _);

        var remaining = MatchmakingContext.Queue.Where(p => p.UserID != currentUser.UserID).ToList();
        MatchmakingContext.Queue.Clear();

        foreach (var p in remaining)
        {
            MatchmakingContext.Queue.Enqueue(p);
        }

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> Wait(ICurrentUser currentUser, HttpContext context)
    {
        if (!MatchmakingContext.Waiters.TryGetValue(currentUser.UserID, out var tcs))
        {
            return TypedResults.BadRequest("Not in matchmaking queue.");
        }

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            context.RequestAborted, new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token
        );

        try
        {
            var result = await tcs.Task.WaitAsync(linkedCts.Token);

            // Both users will hit this endpoint
            // So currentUser will be user01 and user02
            MatchmakingContext.Waiters.TryRemove(currentUser.UserID, out _);

            return TypedResults.Ok(result.matchID.ToString());
        }
        catch (OperationCanceledException)
        {
            MatchmakingContext.Waiters.TryRemove(currentUser.UserID, out _);

            var remaining = MatchmakingContext.Queue.Where(p => p.UserID != currentUser.UserID).ToList();
            MatchmakingContext.Queue.Clear();

            foreach (var p in remaining)
            {
                MatchmakingContext.Queue.Enqueue(p);
            }

            return TypedResults.Ok("timeout_or_cancelled");
        }
    }
}
