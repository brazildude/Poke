// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Moq;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.Data.Player;
using Poke.Server.Data.Player.Models;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared.Enums;
using Poke.Debug;
using static Poke.Server.Endpoints.PlayEndpoints;
using static Poke.Server.Endpoints.TeamEndpoints;
using static Poke.Server.Endpoints.UserEndpoints;
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


        var matchID = Guid.Parse("e64f820e-01a6-488d-96e3-b37f06554589");
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

        var result = Play(playVM, currentUser, playerContext);
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

        matchContext.SaveChanges();
    }
}
