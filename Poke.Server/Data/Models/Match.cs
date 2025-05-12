using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public class Match
{
    public int MatchID { get; set; }
    public int Team01ID { get; set; }
    public int Team02ID { get; set; }

    public string CurrentUserID { get; set; } = null!;
    public int Round { get; set; }
    public int RandomSeed { get; set; }

    public Team Team01 { get; set; } = null!;
    public Team Team02 { get; set; } = null!;

    public void Play(Unit unitInAction, Skill skill, HashSet<int> targetIDs)
    {
        if (!unitInAction.IsAlive())
        {
            return;
        }

        if (skill.IsInCooldown())
        {
            return;
        }

        if (!unitInAction.CheckSkillCost(skill))
        {
            return;
        }

        var ownUnits = GetCurrentTeam(CurrentUserID).Units;
        var enemyUnits = GetEnemyTeam(CurrentUserID).Units;

        if (!AreTargetsValid(skill, ownUnits, enemyUnits, targetIDs))
        {
            return;
        }

        unitInAction.UseSkill(skill, ownUnits, enemyUnits, targetIDs, RandomSeed);
    }

    public Team GetCurrentTeam(string userID)
    {
        if (Team01.UserID == userID)
        {
            return Team01;
        }

        return Team02;
    }

    public Team GetEnemyTeam(string userID)
    {
        if (Team01.UserID == userID)
        {
            return Team02;
        }

        return Team01;
    }

    public virtual bool AreTargetsValid(Skill skill, List<Unit> ownUnits, List<Unit> enemyUnits, HashSet<int> targetIDs)
    {
        var targetType = skill.Target.TargetType;
        var targetDirection = skill.Target.TargetDirection;

        // Always valid for Random and Self target types
        if (targetType == TargetType.Random || targetType == TargetType.Self || targetType == TargetType.All)
            return true;

        // Cannot be valid if no targets are selected
        if (targetIDs.Count == 0)
            return false;

        var ownUnitIds = ownUnits.Where(u => u.IsAlive()).Select(u => u.UnitID).ToHashSet();
        var enemyUnitIds = enemyUnits.Where(u => u.IsAlive()).Select(u => u.UnitID).ToHashSet();
        var allUnitIds = ownUnitIds.Union(enemyUnitIds);

        // Ensure all selected targets are valid units
        if (!targetIDs.All(id => allUnitIds.Contains(id)))
            return false;

        // Check for Select target constraints
        if (targetType == TargetType.Select)
        {
            if (targetIDs.Count > skill.Target.Quantity)
                return false;

            if ((targetDirection == TargetDirection.Own && targetIDs.Any(enemyUnitIds.Contains)) ||
                (targetDirection == TargetDirection.Enemy && targetIDs.Any(ownUnitIds.Contains)))
            {
                return false;
            }
        }

        return true;
    }
}
