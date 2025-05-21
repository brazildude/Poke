using System.Collections.Concurrent;
using Poke.Server.Data.Models;

namespace Poke.Server.Infrastructure.Matchmaking;

public static class MatchmakingState
{
    public record WaitingPlayer(string UserID, int TeamID, TaskCompletionSource<(Guid matchID, string role)> Tcs);

    public static ConcurrentQueue<WaitingPlayer> Queue = new();
    public static ConcurrentDictionary<string, TaskCompletionSource<(Guid matchID, string role)>> Waiters = new();
    public static ConcurrentDictionary<Guid, Match> Matches = new();
}
