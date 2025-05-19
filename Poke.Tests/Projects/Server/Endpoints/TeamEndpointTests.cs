using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Endpoints;
using Poke.Server.Infrastructure.Auth;
using Poke.Tests.Infrastructure;
using static Poke.Server.Endpoints.TeamEndpoints;

namespace Poke.Tests.Projects.Server.Endpoints;

public class TeamEndpointTests : BaseIntegratedTest
{
    [Fact]
    public void ShouldReturnBadRequestWhenUnitsIsNot4()
    {
        // arrange
        var createTeamVM = new CreateTeamVM("Team 01", new HashSet<string> { "" });
        var currentUser = new CurrentUser("01");

        // act
        var response = CreateTeam(createTeamVM, currentUser, null!);
        var result = response.Result as BadRequest<string>;

        // assert
        Assert.NotNull(result);
        Assert.Equal("You must select 4 units.", result.Value);
    }
}
