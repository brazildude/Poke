using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;

namespace Poke.Tests.Infrastructure;

public abstract class BaseIntegratedTest
{
    protected PokeDbContext CreateContext([CallerMemberName] string callerMemberName = "")
    {
        var fileName = $"{AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\net9.0\\", "\\Infrastructure\\DBs\\")}{GetType().Name}.{callerMemberName}.db";
        var connectionstring = $"Data Source={fileName};";
        var optionsBuilder = new DbContextOptionsBuilder<PokeDbContext>();
        optionsBuilder.UseSqlite(connectionstring);

        var pokeContext = new PokeDbContext(optionsBuilder.Options);

        File.Delete(fileName);
        pokeContext.Database.EnsureCreated();

        return pokeContext;
    }
}
