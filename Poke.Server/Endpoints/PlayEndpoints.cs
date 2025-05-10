using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Data;
using Poke.Server.Data.Models;

namespace Poke.Server.Endpoints;

public static class PlayEndpoints
{
    public static void RegisterPlayEndpoints(this WebApplication app)
    {
        var userEndpoints = app.MapGroup("api/plays")
        .RequireAuthorization()
        .RequireCors("_myAllowSpecificOrigins");

        userEndpoints.MapGet("", GetPlay);
        userEndpoints.MapPost("", CreatePlay);
    }

    public static async Task<Results<Ok<User>, NotFound>> GetPlay(int userID, PokeContext db) 
    {
        var user = await db.Users
        .Include(x => x.Teams).ThenInclude(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.SkillCost)
        .Include(x => x.Teams).ThenInclude(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.ApplyValue)
        .SingleOrDefaultAsync(x => x.UserID == userID);

        if (user == null)
        {
            return TypedResults.NotFound();
        }
            
        return TypedResults.Ok(user);
    }

    public static async Task<Results<Ok<string>, Ok>> CreatePlay(PlayDTO playDTO, PokeContext db) 
    {
        // if (db.Users.Any(x => x.Name == name))
        // {
        //     return TypedResults.Ok("Name already exists.");
        // }
// 
        // var user = new User 
        // { 
        //     Name = name,
        //     Teams = new List<Team>
        //     {
        //         new Team { Units = new List<BaseUnit> { new Mage(), new Warrior() }}
        //     }
        // };
        // 
        // db.Users.Add(user);
        // await db.SaveChangesAsync();

        return TypedResults.Ok();
    }

    public class PlayDTO
    {
        public int UnitID { get; set; }
        public int SkillID { get; set; }
        public List<int> TargetIDs { get; set; } = new List<int>();
    }
}
