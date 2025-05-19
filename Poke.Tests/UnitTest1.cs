using Poke.Server.Data.Models;
using Poke.Tests.Infrastructure;

namespace Poke.Tests;

public class UnitTest1 : BaseIntegratedTest
{
    [Fact]
    public void Test1()
    {
        var pokeContext = CreateContext();
        
        pokeContext.Users.Add(new User { UserID = "001" });
        pokeContext.SaveChanges();
        Assert.True(true);
    }

    [Fact]
    public void Test2()
    {
        var pokeContext = CreateContext();

        pokeContext.Users.Add(new User { UserID = "001" });
        pokeContext.SaveChanges();
        Assert.True(true);
    }
}
