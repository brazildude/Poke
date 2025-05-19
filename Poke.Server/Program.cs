using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Endpoints;
using Poke.Server.Infrastructure.Auth;

[assembly: InternalsVisibleTo("Poke.Debug")]
[assembly: InternalsVisibleTo("Poke.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PokeContext>(builder.Configuration["DatabaseProvider"] switch
{
    "sqlite" => opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")),
    "sqlserver" => opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    _ => throw new Exception("Invalid DatabaseProvider")
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("Firebase"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: builder.Configuration["Cors:Name"]!,
    configuration =>
    {
        configuration.WithOrigins(builder.Configuration["Cors:FrontendOrigin"]!)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
builder.Services.AddAuthentication("Firebase").AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", null);
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var serviceScope = app.Services.CreateScope())
    using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<PokeContext>())
    {
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
    }
}
else
{
    // app.UseHttpsRedirection();
}

app.MapStaticAssets().AllowAnonymous();
app.MapOpenApi().AllowAnonymous();
app.UseFirebase();
app.UseCors(builder.Configuration["Cors:Name"]!);
app.UseAuthentication();
app.UseAuthorization();

app.UseDeveloperExceptionPage();

app.RegisterMatchmakingEndpoints();
app.RegisterPlayEndpoints();
app.RegisterTeamEndpoints();
app.RegisterUnitEndpoints();
app.RegisterUserEndpoints();

app.Run();