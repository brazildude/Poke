using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
#if DEBUG
#else
//builder.Services.AddDbContext<PokeContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
#endif

builder.Services.AddDbContext<PokeContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.MapOpenApi();
    app.UseStaticFiles();

    using (var serviceScope = app.Services.CreateScope())
    using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<PokeContext>())
    {
       dbContext.Database.EnsureDeleted();
       dbContext.Database.EnsureCreated();
    }
//}

app.UseHttpsRedirection();

app.RegisterUserEndpoints();

app.Run();