using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Poke.Server.Cache;
using Poke.Server.Data.Match;
using Poke.Server.Data.Match.Models;
using Poke.Server.Data.Player;
using Poke.Server.GameLogic;
using Poke.Server.Infrastructure.Auth;
using Poke.Server.Shared.Mappers;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Endpoints;

public static class MatchEndpoints
{
    public static void RegisterMatchEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("api/matches")
        .RequireAuthorization()
        .RequireCors();

        endpoints.MapPost("play", Play);
    }

    public static Results<Ok<PlayOutputVM>, BadRequest<string>, NotFound> Play(PlayVM playVM, ICurrentUser currentUser, MatchContext matchContext)
    {
        // TODO: this code need a lock on matchState
        if (CacheContext.Matches.TryGetValue(playVM.MatchID, out var matchState))
        {
            if (currentUser.UserID != matchState.CurrentUserID)
            {
                return TypedResults.BadRequest("It's not your turn.");
            }

            if (!matchState.GetCurrentTeam().TryGetValue(playVM.UnitID, out var unitInAction))
            {
                return TypedResults.BadRequest("Unit not found in your team.");
            }

            if (!unitInAction.Skills.TryGetValue(playVM.SkillName, out var skillInAction))
            {
                return TypedResults.BadRequest("Skill not found in unit.");
            }

            return MatchLogic.HandlePlay(matchState, unitInAction, skillInAction, playVM.TargetIDs)
                .Match<Results<Ok<PlayOutputVM>, BadRequest<string>, NotFound>>(
                _continue =>
                {
                    var turnEvents = matchState.GetTurnEvents();
                    var playOutputVM = new PlayOutputVM(currentUser.UserID, unitInAction.Name.ToString(), skillInAction.Name.ToString(), playVM.TargetIDs, turnEvents);
                    return TypedResults.Ok(playOutputVM);
                },
                matchFinished =>
                {
                    var match = matchContext.Matches.Single(x => x.MatchID == playVM.MatchID);
                    match.IsMatchOver = true;
                    match.UserWinnerID = matchFinished.WinnerPlayerID;
                    match.State = matchState;

                    matchContext.Matches.Update(match);
                    matchContext.SaveChanges();

                    CacheContext.Matches.TryRemove(playVM.MatchID, out _);

                    var turnEvents = matchState.GetTurnEvents();
                    var playOutputVM = new PlayOutputVM(currentUser.UserID, unitInAction.Name.ToString(), skillInAction.Name.ToString(), playVM.TargetIDs, turnEvents);
                    return TypedResults.Ok(playOutputVM);
                },
                error => TypedResults.BadRequest(error.Message)
            );
        }

        return TypedResults.NotFound();
    }

    public static Results<Ok<MatchState>, BadRequest<string>> GetMatchState(Guid matchID)
    {
        if (CacheContext.Matches.TryGetValue(matchID, out var matchState))
        {
            // TODO: do I need to create a viewmodel for MatchState?
            return TypedResults.Ok(matchState);
        }

        return TypedResults.BadRequest("Match not found.");
    }

    // TODO: create a method to get the last play of a match with its events

    public static Results<Ok<Guid>, BadRequest<string>> CreateMatch(CreateMatchVM createMatchVM, MatchContext matchContext, PlayerContext playerContext)
    {
        var randomUser = Random.Shared.Next(0, 2);
        var matchID = Guid.NewGuid();
        var match = new Match
        {
            MatchID = matchID,
            IsMatchOver = false,
            UserID01 = createMatchVM.UserID01,
            UserID02 = createMatchVM.UserID02,
            State = new MatchState
            {
                MatchID = matchID,
                CurrentUserID = randomUser == 0 ? createMatchVM.UserID01 : createMatchVM.UserID02,
                EnemyUserID = randomUser == 0 ? createMatchVM.UserID02 : createMatchVM.UserID01,
                Random = new Random(Environment.TickCount),
                Round = 1,
            },
        };

        var playerTeam01 = GetTeam(createMatchVM.Team1ID, playerContext);
        var playerTeam02 = GetTeam(createMatchVM.Team2ID, playerContext);

        var team01 = PlayerMapper.ToMatchTeam(playerTeam01);
        var team02 = PlayerMapper.ToMatchTeam(playerTeam02);

        match.State.Teams.Add(createMatchVM.UserID01, team01);
        match.State.Teams.Add(createMatchVM.UserID02, team02);

        if (!CacheContext.Matches.TryAdd(match.MatchID, match.State))
        {
            return TypedResults.BadRequest("Failed to create match.");
        }

        matchContext.Matches.Add(match);
        matchContext.SaveChanges();

        return TypedResults.Ok(match.MatchID);
    }

    private static IQueryable<Data.Player.Models.Unit> GetTeam(int teamID, PlayerContext playerContext)
    {
        return playerContext.Teams
            .Include(x => x.Units).ThenInclude(x => x.FlatProperties)
            .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.FlatProperties)
            .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.MinMaxProperties)
            .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Target)
            .Include(x => x.Units).ThenInclude(x => x.Skills).ThenInclude(x => x.Behaviors).ThenInclude(x => x.Costs).ThenInclude(x => x.FlatProperty)
            .Where(x => x.TeamID == teamID)
            .AsSplitQuery()
            .AsNoTracking()
            .SelectMany(x => x.Units);
    }
}
