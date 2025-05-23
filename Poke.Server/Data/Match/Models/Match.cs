namespace Poke.Server.Data.Match.Models;

public class Match
{
    public Guid MatchID { get; set; }
    public string UserID01 { get; set; } = null!;
    public string UserID02 { get; set; } = null!;
    public int Team01ID { get; set; }
    public int Team02ID { get; set; }

    public string CurrentUserID { get; set; } = null!;
    public int Round { get; set; }
    public int RandomSeed { get; set; }
    public bool IsMatchOver { get; set; }
    public string? UserWinnerID { get; set; }

    public List<Play> Plays { get; set; } = [];

   // public void Play(Unit unitInAction, Skill skill, HashSet<int> targetIDs)
   // {
   //     if (!unitInAction.IsAlive())
   //     {
   //         return;
   //     }

   //     if (!unitInAction.CanPlay())
   //     {
   //         return;
   //     }

   //     if (skill.IsInCooldown())
   //     {
   //         return;
   //     }

   //     if (!unitInAction.CheckSkillCost(skill))
   //     {
   //         return;
   //     }

   //     var ownUnits = GetCurrentTeam(CurrentUserID).Units;
   //     var enemyUnits = GetEnemyTeam(CurrentUserID).Units;

   //     if (!AreTargetsValid(skill, ownUnits, enemyUnits, targetIDs))
   //     {
   //         return;
   //     }

   //     Plays.Add(new Play
   //     {
   //         TeamID = unitInAction.TeamID,
   //         UnitInActionID = unitInAction.UnitID,
   //         SkillID = skill.UnitID,
   //         TargetIDs = targetIDs
   //     });

   //     unitInAction.UseSkill(skill, ownUnits, enemyUnits, targetIDs, RandomSeed);

   //     CheckMatchOver();

   //     if (!IsMatchOver)
   //     {
   //         var allUnits = Team01.Units.Concat(Team02.Units);
   //         var allAliveUnits = allUnits.Where(x => x.IsAlive());
   //         if (IsRoundOver(allAliveUnits))
   //         {
   //             Round += 1;
   //             foreach (var aliveUnit in allAliveUnits)
   //             {
   //                 TickCooldown(aliveUnit, skill);
   //                 aliveUnit.Properties.Single(x => x.PropertyName == PropertyName.PlayTimes).SetCurrentToBase();
   //             }
   //         }
   //         
   //         // changing current user
   //         CurrentUserID = Team01.UserID == CurrentUserID ? Team02.UserID : Team01.UserID;
   //     }
   // }

   // public void TickCooldown(Unit aliveUnit, Skill skill)
   // {
   //     var cooldowns = aliveUnit.Skills
   //         .Where(x => x.SkillID != skill.SkillID)
   //         .SelectMany(x => x.Behaviors)
   //         .SelectMany(x => x.Properties
   //             .Where(p => p.PropertyName == PropertyName.Cooldown && p.CurrentValue > 0)
   //         );

   //     foreach (var cooldown in cooldowns)
   //     {
   //         cooldown.CurrentValue -= 1;
   //     }
   // }

   // public bool IsRoundOver(IEnumerable<Unit> allAliveUnits)
   // {
   //     if (allAliveUnits.Any(x => x.CanPlay()))
   //     {
   //         return false;
   //     }

   //     return true;
   // }

   // public void CheckMatchOver()
   // {
   //     if (Team01.Units.Any(x => x.IsAlive()) && Team02.Units.All(x => !x.IsAlive()))
   //     {
   //         IsMatchOver = true;
   //         UserWinnerID = Team01.UserID;
   //     }

   //     if (Team02.Units.Any(x => x.IsAlive()) && Team01.Units.All(x => !x.IsAlive()))
   //     {
   //         IsMatchOver = true;
   //         UserWinnerID = Team02.UserID;
   //     }

   //     if (Team01.Units.All(x => !x.IsAlive()) && Team02.Units.All(x => !x.IsAlive()))
   //     {
   //         IsMatchOver = true;
   //         UserWinnerID = null;
   //     }
   // }

   // public Team GetCurrentTeam(string userID)
   // {
   //     if (Team01.UserID == userID)
   //     {
   //         return Team01;
   //     }

   //     return Team02;
   // }

   // public Team GetEnemyTeam(string userID)
   // {
   //     if (Team01.UserID == userID)
   //     {
   //         return Team02;
   //     }

   //     return Team01;
   // }

   // public bool AreTargetsValid(Skill skill, List<Unit> ownUnits, List<Unit> enemyUnits, HashSet<int> targetIDs)
   // {
   //     foreach (var behavior in skill.Behaviors)
   //     {
   //         var targetType = behavior.Target.TargetType;
   //         var targetDirection = behavior.Target.TargetDirection;

   //         // Always valid for Random and Self target types
   //         if (targetType == TargetType.Random || targetType == TargetType.Self || targetType == TargetType.All)
   //             break;

   //         // Cannot be valid if no targets are selected
   //         if (targetIDs.Count == 0)
   //             return false;

   //         var ownUnitIds = ownUnits.Where(u => u.IsAlive()).Select(u => u.UnitID).ToHashSet();
   //         var enemyUnitIds = enemyUnits.Where(u => u.IsAlive()).Select(u => u.UnitID).ToHashSet();
   //         var allUnitIds = ownUnitIds.Union(enemyUnitIds);

   //         // Ensure all selected targets are valid units
   //         if (!targetIDs.All(id => allUnitIds.Contains(id)))
   //             return false;

   //         // Check for Select target constraints
   //         if (targetType == TargetType.Select)
   //         {
   //             if (targetIDs.Count > behavior.Target.Quantity)
   //                 return false;

   //             if ((targetDirection == TargetDirection.Own && targetIDs.Any(enemyUnitIds.Contains)) ||
   //                 (targetDirection == TargetDirection.Enemy && targetIDs.Any(ownUnitIds.Contains)))
   //             {
   //                 return false;
   //             }
   //         }
   //     }

   //     return true;
   // }
}
