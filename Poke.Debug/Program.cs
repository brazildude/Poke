// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.Data.Player;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared.Enums;
using Poke.Debug;
using static Poke.Server.Endpoints.PlayEndpoints;
using static Poke.Server.Infrastructure.ViewModels;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting Match context");

        SimulateMatch();
    }

    private static void SimulateMatch()
    {
        var connectionstring = "Data Source=Poke.db;";

        var playerContextOptions = new DbContextOptionsBuilder<PlayerContext>().UseSqlite(connectionstring).Options;
        var playerContext = new PlayerContext(playerContextOptions);

        var matchContextOptions = new DbContextOptionsBuilder<MatchContext>().UseSqlite(connectionstring).Options;
        var matchContext = new MatchContext(matchContextOptions);

        var matchID = Guid.Parse("f314d9d5-ee75-41bb-b4cc-c558bb2a5a32");
        var nM = matchContext.Matches.Single(x => x.MatchID == matchID);
        var a = nM;

        matchContext.Database.EnsureDeleted();

        playerContext.Database.Migrate();
        matchContext.Database.Migrate();

        var match = MatchGenerator.CreateMatch();
        matchContext.Matches.Add(match);
        matchContext.SaveChanges();

        MatchmakingContext.Matches.TryAdd(match.MatchID, match);
        var playVM = new PlayVM(match.MatchID, 1, SkillName.Cleave, new HashSet<int> { 5 });
        var currentUser = new CurrentUser("UserID01", null, null, null);

        var result = Play(playVM, currentUser);
        if (result.Result is Ok<PlayVM> ok)
        {
            Console.WriteLine(ok.Value);

            Console.WriteLine("Match State Events:");
            foreach (var e in match.State.GetEventsSince(0))
            {
                Console.WriteLine(e.Type);
            }
        }

        if (result.Result is BadRequest bad)
        {
            Console.WriteLine(bad.StatusCode);
        }
    }
}
