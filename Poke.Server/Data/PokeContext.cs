using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Models;

namespace Poke.Server.Data;

public class PokeContext : DbContext
{
    public PokeContext(DbContextOptions<PokeContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
}
