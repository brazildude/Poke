using System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;

namespace Poke.Server.Endpoints;

public static class UserEndpoints
{
    public static void RegisterUserEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("/users");

        userEndpoints.MapGet("{userID}", GetUser);
        userEndpoints.MapPost("{name}", CreateUser);
        userEndpoints.MapGet("test", Test);
    }

    static async Task<Results<Ok<User>, NotFound>> GetUser(int userID, PokeContext db) 
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.UserID == userID);

        if (user == null)
        {
            return TypedResults.NotFound();
        }
            
        return TypedResults.Ok(user);
    }

    static async Task<Results<Ok<string>, Ok>> CreateUser(string name, PokeContext db) 
    {
        if (db.Users.Any(x => x.Name == name))
        {
            return TypedResults.Ok("Name already exists.");
        }

        db.Users.Add(new User { Name = name });
        await db.SaveChangesAsync();

        return TypedResults.Ok();
    }

    static async Task<string> Test()
    {
        return "OK";
    }
}
