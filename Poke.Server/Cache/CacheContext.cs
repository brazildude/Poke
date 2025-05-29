using System.Collections.Concurrent;
using Poke.Server.Data.Match.Models;

namespace Poke.Server.Cache;

public class CacheContext
{
    public static ConcurrentDictionary<Guid, MatchState> Matches = new();
}