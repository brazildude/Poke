// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Moq;
using Poke.Server.Cache;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Endpoints.PlayEndpoints;
using static Poke.Server.Endpoints.TeamEndpoints;
using static Poke.Server.Endpoints.UserEndpoints;
using static Poke.Server.Infrastructure.ViewModels;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Match context");

        await SimulateMatch();
    }

    private static async Task SimulateMatch()
    {
        var connectionstring = "Data Source=Poke.db;";
        var optionsBuilder = new DbContextOptionsBuilder<PokeDbContext>();
        optionsBuilder.UseSqlite(connectionstring);

        var pokeContext = new PokeDbContext(optionsBuilder.Options);
        pokeContext.Database.EnsureDeleted();
        pokeContext.Database.EnsureCreated();

        var authMock = new Mock<IAuthService>();

        authMock.SetupSequence(x => x.VerifyIdTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("01")!)
                .Returns(Task.FromResult("02")!);

        await CreateUser(new CreateUserVM("Google", "0001"), authMock.Object, pokeContext);
        await CreateUser(new CreateUserVM("Google", "0001"), authMock.Object, pokeContext);

        var user01 = pokeContext.Users.Include(x => x.Teams).ThenInclude(x => x.Units).Single(x => x.UserID == "01");
        var user02 = pokeContext.Users.Include(x => x.Teams).ThenInclude(x => x.Units).Single(x => x.UserID == "02");

        var unitNames = new HashSet<string> { "Mage", "Warrior", "Rogue", "Paladin" };

        CreateTeam(new CreateTeamVM("My Team 01", unitNames), new CurrentUser("01"), pokeContext);
        CreateTeam(new CreateTeamVM("My Team 02", unitNames), new CurrentUser("02"), pokeContext);

        var match = new Poke.Server.Data.Models.Match
        {
            CurrentUserID = user01.UserID,
            Team01 = SelectTeam(1, pokeContext),
            Team02 = SelectTeam(2, pokeContext),
            Round = 1,
            RandomSeed = 10000
        };
        pokeContext.Matches.Add(match);
        pokeContext.SaveChanges();

        MatchmakingContext.Matches.TryAdd(match.MatchID, match);
        
        var result = Play(new PlayVM(match.MatchID, 1, 3, new HashSet<int> { 5 }), new CurrentUser("01", null, null, null), pokeContext);
        if (result.Result is Ok<PlayVM> ok)
        {
            Console.WriteLine(ok.Value);
        }
        
        if (result.Result is BadRequest bad)
        {
            Console.WriteLine(bad.StatusCode);
        }
    }

    private static Team SelectTeam(int teamID, PokeDbContext db)
    {
        return db.Teams.Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.MinMaxProperty)
                .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Target)
                .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Costs).ThenInclude(x => x.FlatProperty)
                .Single(x => x.TeamID == teamID);
    }
}
