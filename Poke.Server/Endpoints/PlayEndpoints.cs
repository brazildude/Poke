using System.Text.Json;
using Force.DeepCloner;
using Microsoft.AspNetCore.Http.HttpResults;
using Poke.Server.Data;
using Poke.Server.Infrastructure;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Infrastructure.Matchmaking;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class PlayEndpoints
{
    public static void RegisterPlayEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/plays")
        .RequireAuthorization()
        .RequireCors();

        endpoints.MapPost("", Play);
    }

    public static Results<Ok<PlayVM>, NotFound, BadRequest> Play(PlayVM playVM, ICurrentUser currentUser, PokeContext db)
    {
        if (!MatchmakingState.Matches.TryGetValue(playVM.MatchID, out var match))
        {
            return TypedResults.NotFound();
        }

        if (currentUser.UserID != match.CurrentUserID)
        {
            return TypedResults.BadRequest();
        }

        var unitInAction = match.GetCurrentTeam(currentUser.UserID).Units.SingleOrDefault(x => x.UnitID == playVM.UnitID);

        if (unitInAction == null)
        {
            return TypedResults.BadRequest();
        }

        var skill = unitInAction.Skills.SingleOrDefault(x => x.SkillID == playVM.SkillID);

        if (skill == null)
        {
            return TypedResults.BadRequest();
        }

        var initialState = match.DeepClone();
        match.Play(unitInAction, skill, playVM.TargetIDs);

        db.SaveChanges();

        var changes = UltraFastObjectDiff.GetChanges(initialState, match, new HashSet<string> { "Plays" });
        Console.WriteLine(JsonSerializer.Serialize(changes));

        return TypedResults.Ok(playVM);
    }
}
