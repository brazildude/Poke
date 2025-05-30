using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Match;
using Poke.Server.Data.Player;
using Poke.Server.Endpoints;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Infrastructure.Auth.Firebase;
using Poke.Server.Infrastructure.Auth.Local;

[assembly: InternalsVisibleTo("Poke.Debug")]
[assembly: InternalsVisibleTo("Poke.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PlayerContext>(builder.Configuration["DatabaseProvider"] switch
{
    "sqlite" => opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")),
    "sqlserver" => opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    _ => throw new Exception("Invalid DatabaseProvider")
});
builder.Services.AddDbContext<MatchContext>(builder.Configuration["DatabaseProvider"] switch
{
    "sqlite" => opt => opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")),
    "sqlserver" => opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
    _ => throw new Exception("Invalid DatabaseProvider")
});

builder.Services.Configure<JsonOptions>(options =>
{
    //options.SerializerOptions.Converters.Add(new TimeSpanConverter());
    //options.SerializerOptions.Converters.Add(new JsonDateTimeConverter());
    //options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(builder.Configuration["Cors:FrontendOrigin"]!)
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
{
    builder.Services.AddScoped<IAuthService, FireAuthService>();
    builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("Firebase"));
    builder.Services.AddAuthentication("Firebase").AddScheme<AuthenticationSchemeOptions, FirebaseAuthenticationHandler>("Firebase", null);
}
else
{
    builder.Services.AddScoped<IAuthService, LocalAuthService>();
    builder.Services.AddAuthentication("Local").AddScheme<AuthenticationSchemeOptions, LocalAuthenticationHandler>("Local", null);
}

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var serviceScope = app.Services.CreateScope())
    using (var dbContext = serviceScope.ServiceProvider.GetRequiredService<PlayerContext>())
    {
        //dbContext.Database.EnsureDeleted();
        //dbContext.Database.EnsureCreated();
    }
}
else
{
    // app.UseHttpsRedirection();
}

app.MapStaticAssets().AllowAnonymous();
app.MapOpenApi().AllowAnonymous();
app.UseFirebase();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseDeveloperExceptionPage();

app.RegisterMatchmakingEndpoints();
app.RegisterPlayEndpoints();
app.RegisterSkillEndpoints();
app.RegisterTeamEndpoints();
app.RegisterUnitEndpoints();
app.RegisterUserEndpoints();

app.Run();