// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;
using Poke.Server.Data.Models.Units;
using Poke.Server.Endpoints;
using Poke.Server.Infrastructure.Auth;

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

        await UserEndpoints.CreateUser(new CurrentUser(new HttpContextAccessor()) { ExternalUserID = "Google|0001" }, pokeContext);
        await UserEndpoints.CreateUser(new CurrentUser(new HttpContextAccessor()) { ExternalUserID = "Google|0002" }, pokeContext);

        var user01 = pokeContext.Users.Single(x => x.UserID == 1);
        var user02 = pokeContext.Users.Single(x => x.UserID == 2);

        var match = new Match
        {
            CurrentTeamID = user01.UserID,
            CurrentTeam = user01.Teams.Single(),
            NextTeam = user02.Teams.Single(),
            Round = 1
        };

        var unit = match.CurrentTeam.Units[0];

        unit.UseSkill(unit.Skills.First().BaseSkillID, match.CurrentTeam.Units, match.NextTeam.Units);
    }
}
