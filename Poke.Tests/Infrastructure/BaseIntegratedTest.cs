using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Player;

namespace Poke.Tests.Infrastructure;

public abstract class BaseIntegratedTest
{
    protected PlayerContext CreateContext([CallerMemberName] string callerMemberName = "")
    {
        var fileName = $"{AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net9.0\\", "\\Infrastructure\\DBs\\")}{GetType().Name}.{callerMemberName}.db";
        var connectionstring = $"Data Source={fileName};";
        var optionsBuilder = new DbContextOptionsBuilder<PlayerContext>();
        optionsBuilder.UseSqlite(connectionstring);

        var pokeContext = new PlayerContext(optionsBuilder.Options);

        File.Delete(fileName);
        pokeContext.Database.EnsureCreated();

        return pokeContext;
    }
}
