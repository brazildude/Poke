// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Moq;
using Poke.Server.Data;
using Poke.Server.Infrastructure.Auth;
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

        authMock.Setup(x => x.VerifyIdTokenAsync(It.IsAny<string>()))
                .Returns(Task.FromResult("")!);

        await CreateUser(new CreateUserVM("Google", "0001"), authMock.Object, pokeContext);
        await CreateUser(new CreateUserVM("Google", "0001"), authMock.Object, pokeContext);

        var user01 = pokeContext.Users.Include(x => x.Teams).ThenInclude(x => x.Units).Single(x => x.UserID == "0001");
        var user02 = pokeContext.Users.Include(x => x.Teams).ThenInclude(x => x.Units).Single(x => x.UserID == "0002");

        var match = new Poke.Server.Data.Models.Match
        {
            CurrentUserID = user01.UserID,
            Team01 = user01.Teams.Single(),
            Team02 = user02.Teams.Single(),
            Round = 1,
            RandomSeed = Environment.TickCount
        };
        
        var unit = match.GetCurrentTeam(user01.UserID).Units[0];

        unit.UseSkill(unit.Skills.First(), match.Team01.Units, match.Team02.Units, new HashSet<int> { 4 }, match.RandomSeed);
    }
}
