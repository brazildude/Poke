// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Moq;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Endpoints;
using Poke.Server.Infrastructure.Auth;
using static Poke.Server.Endpoints.TeamEndpoints;
using static Poke.Server.Endpoints.UserEndpoints;

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
        var optionsBuilder = new DbContextOptionsBuilder<PokeContext>();
        optionsBuilder.UseSqlite(connectionstring);

        var pokeContext = new PokeContext(optionsBuilder.Options);
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
        
        CreateTeam(new CreateTeamVM("My Team 01", new List<int> { 1, 2, 3, 4 }), new CurrentUser("01", null, null, null), pokeContext);
        CreateTeam(new CreateTeamVM("My Team 02", new List<int> { 1, 2, 3, 4 }), new CurrentUser("02", null, null, null), pokeContext);

        var match = new Poke.Server.Data.Models.Match
        {
            CurrentUserID = user01.UserID,
            Team01 = SelectTeam(1, pokeContext),
            Team02 = SelectTeam(2, pokeContext),
            Round = 1,
            RandomSeed = Environment.TickCount
        };
        
        var unit = match.GetCurrentTeam(user01.UserID).Units[0];

        unit.UseSkill(unit.Skills.First(), match.Team01.Units, match.Team02.Units, new HashSet<int> { 4 }, match.RandomSeed);
    }

    private static Team SelectTeam(int teamID, PokeContext db)
    {
        return db.Teams.Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.ApplyValue)
                .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.SkillCost)
                .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Target)
                .Single(x => x.TeamID == teamID);
    }
}
