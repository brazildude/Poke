using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Data.Player.Models;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared.Enums;
using Poke.Tests.Infrastructure;
using static Poke.Server.Endpoints.TeamEndpoints;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Tests.Projects.Server.Endpoints;

public class TeamEndpointTests : BaseIntegratedTest
{
    [Fact]
    public void CreatTeam_BadRequest_TotalUnitsIsNot4()
    {
        // arrange
        var createTeamVM = new CreateTeamVM("Team 01", [(UnitName)1000]);
        var currentUser = new CurrentUser("01");

        // act
        var response = CreateTeam(createTeamVM, currentUser, null!);
        var result = response.Result as BadRequest<string>;

        // assert
        Assert.NotNull(result);
        Assert.Equal("You must select 4 units.", result.Value);
    }

    [Fact]
    public void CreatTeam_BadRequest_UnitNameIsInvalid()
    {
        // arrange
        var createTeamVM = new CreateTeamVM("Team 01", [(UnitName)1000, UnitName.Mage, UnitName.Lancer, UnitName.Rogue]);
        var currentUser = new CurrentUser("01");

        // act
        var response = CreateTeam(createTeamVM, currentUser, null!);
        var result = response.Result as BadRequest<string>;

        // assert
        Assert.NotNull(result);
        Assert.Equal("Invalid unit name.", result.Value);
    }

    [Fact]
    public void CreatTeam_BadRequest_TeamNameAlreadyExists()
    {
        // arrange
        var userID = "01";
        var teamName = "Team 01";
        var createTeamVM = new CreateTeamVM(teamName,  [UnitName.Warlock, UnitName.Mage, UnitName.Lancer, UnitName.Rogue]);
        var currentUser = new CurrentUser(userID);

        var pokeContext = CreateContext();
        var user = new User { UserID = userID, Teams = [new Team { Name = teamName }] };
        pokeContext.Users.Add(user);
        pokeContext.SaveChanges();

        // act
        var response = CreateTeam(createTeamVM, currentUser, pokeContext);
        var result = response.Result as BadRequest<string>;

        // assert
        Assert.NotNull(result);
        Assert.Equal("Team name already exists.", result.Value);
    }

    [Fact]
    public void CreatTeam_Ok()
    {
        // arrange
        var userID = "01";
        var teamName = "Team 01";
        var createTeamVM = new CreateTeamVM(teamName, [UnitName.Warlock, UnitName.Mage, UnitName.Lancer, UnitName.Rogue]);
        var currentUser = new CurrentUser(userID);

        var pokeContext = CreateContext();
        var user = new User { UserID = userID };
        pokeContext.Users.Add(user);
        pokeContext.SaveChanges();

        // act
        var response = CreateTeam(createTeamVM, currentUser, pokeContext);
        var result = response.Result as Ok;

        // assert
        Assert.NotNull(result);
        
        Assert.True(pokeContext.Users.Any(x => x.UserID == userID));
        Assert.True(pokeContext.Teams.Any(x => x.UserID == userID && x.Name == teamName));
    }
}
