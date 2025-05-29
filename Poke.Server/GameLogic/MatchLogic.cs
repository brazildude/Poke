using OneOf;
using Poke.Server.Data.Match.Models;
using Poke.Server.Infrastructure.GameLogic;
using Poke.Server.Shared.Enums;

namespace Poke.Server.GameLogic;

public class MatchLogic
{
    public static OneOf<bool, MatchFinishedDTO, ErrorDTO> HandlePlay(MatchState matchState, Unit unitInAction, Skill skillInAction, HashSet<int> targetIDs)
    {
        if (!UnitLogic.IsAlive(unitInAction))
        {
            return ErrorDTO.New("Unit is not alive.");
        }

        if (!UnitLogic.CanPlay(unitInAction))
        {
            return ErrorDTO.New("Unit cannot play at this time.");
        }

        if (!UnitLogic.HasInitialSkillResources(unitInAction, skillInAction))
        {
            return ErrorDTO.New("Unit does not have enough resources to play this skill.");
        }

        if (SkillLogic.IsInCooldown(skillInAction))
        {
            return ErrorDTO.New("Skill is in cooldown.");
        }

        if (!SkillLogic.AreTargetsValid(skillInAction, matchState.GetCurrentTeam(), matchState.GetEnemyTeam(), targetIDs))
        {
            return ErrorDTO.New("Invalid targets for the skill.");
        }

        matchState.AddPlay(new Play
        {
            UserID = matchState.CurrentUserID,
            UnitInActionID = unitInAction.UnitID,
            SkillID = skillInAction.SkillID,
            TargetIDs = targetIDs,
        });

        UnitLogic.UseSkill(matchState, unitInAction, skillInAction, targetIDs);

        if (CheckMatchOver(matchState, out var userWinnerID))
        {
            return MatchFinishedDTO.New(userWinnerID);
        }

        var allUnits = matchState.GetCurrentTeam().Values.Concat(matchState.GetEnemyTeam().Values);
        var allAliveUnits = allUnits.Where(x => UnitLogic.IsAlive(x));

        if (IsRoundOver(allAliveUnits))
        {
            matchState.Round += 1;
            foreach (var aliveUnit in allAliveUnits)
            {
                SkillLogic.TickCooldown(aliveUnit, skillInAction.Name);
                aliveUnit.FlatProperties[PropertyName.PlayTimes].Reset();
            }
        }

        // changing current user
        var temp = matchState.CurrentUserID;
        matchState.CurrentUserID = matchState.EnemyUserID;
        matchState.EnemyUserID = temp;

        return true;
    }

    public static bool IsRoundOver(IEnumerable<Unit> allAliveUnits)
    {
        if (allAliveUnits.Any(x => UnitLogic.CanPlay(x)))
        {
            return false;
        }

        return true;
    }

    public static bool CheckMatchOver(MatchState matchState, out string? userWinnerID)
    {
        var team01 = matchState.GetCurrentTeam();
        var team02 = matchState.GetEnemyTeam();

        var isMatchOver = false;
        userWinnerID = null;

        if (team01.Any(x => UnitLogic.IsAlive(x.Value)) && team02.All(x => !UnitLogic.IsAlive(x.Value)))
        {
            isMatchOver = true;
            userWinnerID = matchState.CurrentUserID;
        }

        if (team02.Any(x => UnitLogic.IsAlive(x.Value)) && team01.All(x => !UnitLogic.IsAlive(x.Value)))
        {
            isMatchOver = true;
            userWinnerID = matchState.EnemyUserID;
        }

        if (team01.All(x => !UnitLogic.IsAlive(x.Value)) && team02.All(x => !UnitLogic.IsAlive(x.Value)))
        {
            isMatchOver = true;
            userWinnerID = null;
        }

        return isMatchOver;
    }
}
