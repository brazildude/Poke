using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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
        .RequireCors();

        endpoints.MapGet("join", Join);
        endpoints.MapGet("cancel", Cancel);
        endpoints.MapGet("wait", Wait);
    }

    public static Results<Ok<string>, BadRequest<string>> Join(int teamID, ICurrentUser currentUser, PokeContext db)
    {
        // Prevent duplicate join
        if (MatchmakingState.Waiters.ContainsKey(currentUser.UserID))
        {
            return TypedResults.BadRequest("Already in queue.");
        }

        if (!db.Teams.Any(x => x.TeamID == teamID && x.UserID == currentUser.UserID))
        {
            return TypedResults.BadRequest("Team does not exist.");
        }

        var tcs = new TaskCompletionSource<(int, string)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var player = new MatchmakingState.WaitingPlayer(currentUser.UserID, teamID, tcs);

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
            CurrentUserID = Random.Shared.Next(0, 2) == 0 ? currentUser.UserID : opponent.UserID,
            RandomSeed = Environment.TickCount
        };

        var team01 = SelectTeam(player.TeamID, db);
        var team02 = SelectTeam(opponent.TeamID, db);

        if (match.CurrentUserID == player.UserID)
        {
            match.Team01 = team01;
            match.Team02 = team02;
        }
        else
        {
            match.Team01 = team02;
            match.Team02 = team01;
        }

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
            MatchmakingState.Waiters.TryRemove(currentUser.UserID, out _);

            var remaining = MatchmakingState.Queue.Where(p => p.UserID != currentUser.UserID).ToList();
            MatchmakingState.Queue.Clear();

            foreach (var p in remaining)
            {
                MatchmakingState.Queue.Enqueue(p);
            }

            return TypedResults.Ok("timeout_or_cancelled");
        }
    }

    private static Team SelectTeam(int teamID, PokeContext db)
    {
        return db.Teams.Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.MinMaxProperty)
                .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Target)
                .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Costs).ThenInclude(x => x.FlatProperty)
                .Single(x => x.TeamID == teamID);
    }
}
