using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Infrastructure.Matchmaking;

namespace Poke.Server.Endpoints;

public static class MatchmakingEndpoints
{
    public static void RegisterMatchmakingEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/matchmaking")
        .RequireAuthorization()
        .RequireCors("_myAllowSpecificOrigins");

        endpoints.MapGet("join", Join);
        endpoints.MapGet("cancel", Cancel);
        endpoints.MapGet("wait", Wait);
    }

    public static Results<Ok<string>, BadRequest<string>> Join(ICurrentUser currentUser, PokeContext db)
    {
        // Prevent duplicate join
        if (MatchmakingState.Waiters.ContainsKey(currentUser.UserID))
        {
            return TypedResults.BadRequest("Already in queue.");
        }

        var tcs = new TaskCompletionSource<(int, string)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var player = new MatchmakingState.WaitingPlayer(currentUser.UserID, tcs);

        // Try to match
        if (!MatchmakingState.Queue.TryDequeue(out var opponent))
        {
            // Add to queue
            MatchmakingState.Queue.Enqueue(player);
            MatchmakingState.Waiters[currentUser.UserID] = tcs;

            return TypedResults.Ok("Waiting for match...");
        }

        var match = new Match
        {
            User01ID = opponent.UserID
        };

        db.Matches.Add(match);
        db.SaveChanges();
        
        MatchmakingState.Matches.TryAdd(match.MatchID, match);

        opponent.Tcs.TrySetResult((match.MatchID, "player1"));
        tcs.TrySetResult((match.MatchID, "player2"));

        return TypedResults.Ok("Match Found");
    }

    public static Results<Ok<string>, Ok> Cancel(ICurrentUser currentUser)
    {
        MatchmakingState.Waiters.TryRemove(currentUser.UserID, out _);

        var remaining = MatchmakingState.Queue.Where(p => p.UserID != currentUser.UserID).ToList();
        MatchmakingState.Queue.Clear();
        
        foreach (var p in remaining) 
        {
            MatchmakingState.Queue.Enqueue(p);
        }

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> Wait(ICurrentUser currentUser, HttpContext context)
    {
        if (!MatchmakingState.Waiters.TryGetValue(currentUser.UserID, out var tcs))
        {
            return TypedResults.BadRequest("Not in matchmaking queue.");
        }

        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            context.RequestAborted, new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token
        );

        try
        {
            var result = await tcs.Task.WaitAsync(linkedCts.Token);
            MatchmakingState.Waiters.TryRemove(currentUser.UserID, out _);

            return TypedResults.Ok(result.matchID.ToString());
        }
        catch (OperationCanceledException)
        {
            // Timeout or client closed connection — cleanup
            MatchmakingState.Waiters.TryRemove(currentUser.UserID, out _);

            // Remove from queue
            var remaining = MatchmakingState.Queue.Where(p => p.UserID != currentUser.UserID).ToList();
            MatchmakingState.Queue.Clear();
            
            foreach (var p in remaining) 
            {
                MatchmakingState.Queue.Enqueue(p);
            }

            return TypedResults.Ok("timeout_or_cancelled");
        }
    }
}
