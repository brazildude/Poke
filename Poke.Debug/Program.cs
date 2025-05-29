// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.Data.Player;
using Poke.Server.GameLogic.Events;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared.Enums;
using Poke.Debug;
using static Poke.Server.Endpoints.PlayEndpoints;
using static Poke.Server.Infrastructure.ViewModels;

internal class Program
{
    private static void Main(string[] args)
    {
        SimulateMatch();
    }

    private static void SimulateMatch()
    {
        var connectionstring = "Data Source=Poke.db;";

        var playerContextOptions = new DbContextOptionsBuilder<PlayerContext>().UseSqlite(connectionstring).Options;
        var playerContext = new PlayerContext(playerContextOptions);

        var matchContextOptions = new DbContextOptionsBuilder<MatchContext>().UseSqlite(connectionstring).Options;
        var matchContext = new MatchContext(matchContextOptions);

        matchContext.Database.EnsureDeleted();

        playerContext.Database.Migrate();
        matchContext.Database.Migrate();

        var match = MatchGenerator.CreateMatch();
        matchContext.Matches.Add(match);
        matchContext.SaveChanges();

        CacheContext.Matches.TryAdd(match.MatchID, match.State);

        var playVM = new PlayVM(match.MatchID, 1, SkillName.Cleave, new HashSet<int> { 5 });
        var currentUser = new CurrentUser("UserID01", null, null, null);
        var result = Play(playVM, currentUser, matchContext);

        ConsolePlay(result);

        playVM = new PlayVM(match.MatchID, 5, SkillName.Shadowbolt, new HashSet<int> { 1 });
        currentUser = new CurrentUser("UserID02", null, null, null);
        result = Play(playVM, currentUser, matchContext);

        ConsolePlay(result);
    }

    private static void ConsolePlay(Results<Ok<List<GameEvent>>, BadRequest<string>, NotFound> result)
    {
        if (result.Result is Ok<List<GameEvent>> events)
        {

            foreach (var _event in events.Value!)
            {
                if (_event is UnitStateChangedEvent e)
                {
                    Console.WriteLine($"{e.EventId}: {e.Type} - UnitID: {e.UnitID}, Property: {e.PropertyName}, AppliedValue: {e.AppliedValue}, CurrentValue: {e.CurrentValue}, HitType: {e.HitType}");
                    continue;
                }
                
                if (_event is NoResourcesEvent ee)
                {
                    Console.WriteLine($"{ee.EventId}: {ee.Type} - Behavior: {ee.BehaviorName}, Property: {ee.PropertyName}, RequiredValue: {ee.RequiredValue}, CurrentValue: {ee.CurrentValue}");
                    continue;
                }
            }
        }

        if (result.Result is BadRequest<string> bad)
        {
            Console.WriteLine(bad.Value);
        }
    }
}
