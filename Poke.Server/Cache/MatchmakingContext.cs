using System.Collections.Concurrent;
using Poke.Server.Data.Match.Models;

namespace Poke.Server.Cache;

public static class MatchmakingContext
{
    public record WaitingPlayer(string UserID, int TeamID, TaskCompletionSource<(Guid matchID, string role)> Tcs);

    public static ConcurrentQueue<WaitingPlayer> Queue = new();
    public static ConcurrentDictionary<string, TaskCompletionSource<(Guid matchID, string role)>> Waiters = new();
}
