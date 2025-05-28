using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.Data.Match.Models;
using Poke.Server.Data.Player;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared;

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
        // Prevent duplicate join
        if (MatchmakingContext.Waiters.ContainsKey(currentUser.UserID))
        {
            return TypedResults.BadRequest("Already in queue.");
        }

        if (!playerContext.Teams.Any(x => x.TeamID == teamID && x.UserID == currentUser.UserID))
        {
            return TypedResults.BadRequest("Team does not exist.");
        }

        var tcs = new TaskCompletionSource<(Guid, string)>(TaskCreationOptions.RunContinuationsAsynchronously);
        var player = new MatchmakingContext.WaitingPlayer(currentUser.UserID, teamID, tcs);

        // Try to match
        if (!MatchmakingContext.Queue.TryDequeue(out var opponent))
        {
            // Add to queue
            MatchmakingContext.Queue.Enqueue(player);
            MatchmakingContext.Waiters[currentUser.UserID] = tcs;

            return TypedResults.Ok("Waiting for match...");
        }

        var match = new Match
        {
            IsMatchOver = false,
            UserID01 = player.UserID,
            UserID02 = opponent.UserID,
            State = new MatchState
            {
                CurrentUserID = Random.Shared.Next(0, 2) == 0 ? currentUser.UserID : opponent.UserID,
                RandomSeed = Environment.TickCount,
                Round = 1,
            },
        };

        var playerTeam01 = GetTeam(player.TeamID, playerContext);
        var playerTeam02 = GetTeam(player.TeamID, playerContext);

        var team01 = Mapper.ToMatchTeam(playerTeam01);
        var team02 = Mapper.ToMatchTeam(playerTeam02);

        if (match.State.CurrentUserID == player.UserID)
        {
            match.State.Teams.Add(player.UserID, team01);
            match.State.Teams.Add(opponent.UserID, team02);
        }
        else
        {
            match.State.Teams.Add(opponent.UserID, team01);
            match.State.Teams.Add(player.UserID, team02);
        }

        matchContext.Matches.Add(match);
        matchContext.SaveChanges();

        MatchmakingContext.Matches.TryAdd(match.MatchID, match);

        opponent.Tcs.TrySetResult((match.MatchID, "player1"));
        tcs.TrySetResult((match.MatchID, "player2"));

        return TypedResults.Ok("Match Found");
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

    private static IQueryable<Data.Player.Models.Unit> GetTeam(int teamID, PlayerContext playerContext)
    {
        return playerContext.Teams
            .Include(x => x.Units).ThenInclude(x => x.FlatProperties)
            .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.MinMaxProperties)
            .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Target)
            .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Costs).ThenInclude(x => x.FlatProperty)
            .Where(x => x.TeamID == teamID)
            .SelectMany(x => x.Units);
    }
}
