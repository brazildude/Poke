using Poke.Server.Data.Match.Models;
using Poke.Server.Shared.Enums;

namespace Poke.Server.GameLogic;

public class MatchLogic
{
    public static void HandlePlay(Match match, Unit unitInAction, Skill skillInAction, HashSet<int> targetIDs)
    {
        if (!UnitLogic.IsAlive(unitInAction))
        {
            return;
        }

        if (!UnitLogic.CanPlay(unitInAction))
        {
            return;
        }

        if (!UnitLogic.HasInitialSkillResources(unitInAction, skillInAction))
        {
            return;
        }

        if (SkillLogic.IsInCooldown(skillInAction))
        {
            return;
        }

        if (!SkillLogic.AreTargetsValid(skillInAction, match.State.GetCurrentTeam(), match.State.GetEnemyTeam(), targetIDs))
        {
            return;
        }

        match.State.Plays.Add(new Play
        {
            UserID = match.State.CurrentUserID,
            UnitInActionID = unitInAction.UnitID,
            SkillID = skillInAction.SkillID,
            TargetIDs = targetIDs,
            PlayedAt = DateTime.UtcNow
        });

        UnitLogic.UseSkill(match.State, unitInAction, skillInAction, targetIDs);

        CheckMatchOver(match);

        if (!match.IsMatchOver)
        {
            var allUnits = match.State.GetCurrentTeam().Values.Concat(match.State.GetEnemyTeam().Values);
            var allAliveUnits = allUnits.Where(x => UnitLogic.IsAlive(x));

            if (IsRoundOver(allAliveUnits))
            {
                match.State.Round += 1;
                foreach (var aliveUnit in allAliveUnits)
                {
                    SkillLogic.TickCooldown(aliveUnit, skillInAction.Name);
                    aliveUnit.FlatProperties[PropertyName.PlayTimes].Reset();
                }
            }

            // changing current user
            match.State.CurrentUserID = match.UserID01 == match.State.CurrentUserID ? match.UserID02 : match.UserID01;
        }
        else
        {
            //matchContext.Matches.Update(match);
            //matchContext.SaveChanges();
        }
    }

    public static bool IsRoundOver(IEnumerable<Unit> allAliveUnits)
    {
        if (allAliveUnits.Any(x => UnitLogic.CanPlay(x)))
        {
            return false;
        }

        return true;
    }

    public static void CheckMatchOver(Match match)
    {
        var team01 = match.State.GetCurrentTeam();
        var team02 = match.State.GetEnemyTeam();

        if (team01.Any(x => UnitLogic.IsAlive(x.Value)) && team02.All(x => !UnitLogic.IsAlive(x.Value)))
        {
            match.IsMatchOver = true;
            match.UserWinnerID = match.UserID01;
        }

        if (team02.Any(x => UnitLogic.IsAlive(x.Value)) && team01.All(x => !UnitLogic.IsAlive(x.Value)))
        {
            match.IsMatchOver = true;
            match.UserWinnerID = match.UserID02;
        }

        if (team01.All(x => !UnitLogic.IsAlive(x.Value)) && team02.All(x => !UnitLogic.IsAlive(x.Value)))
        {
            match.IsMatchOver = true;
            match.UserWinnerID = null;
        }
    }
}
