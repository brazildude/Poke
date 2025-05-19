using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;

namespace Poke.Tests.Infrastructure;

public abstract class BaseIntegratedTest
{
    protected PokeContext CreateContext([CallerMemberName] string callerMemberName = "")
    {
        var connectionstring = $"Data Source={AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net9.0\\", "\\Infrastructure\\DBs\\")}{GetType().Name}.{callerMemberName}.db;";
        var optionsBuilder = new DbContextOptionsBuilder<PokeContext>();
        optionsBuilder.UseSqlite(connectionstring);

        var pokeContext = new PokeContext(optionsBuilder.Options);
        pokeContext.Database.EnsureCreated();

        return pokeContext;
    }
}
