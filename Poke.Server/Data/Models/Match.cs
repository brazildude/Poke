using static Poke.Server.Endpoints.PlayEndpoints;

namespace Poke.Server.Data.Models;

public class Match
{
    public int MatchID { get; set; }
    public int User01ID { get; set; }
    public int User02ID { get; set; }
    public int CurrentTeamID { get; set; }
    public int Round { get; set; }

    public int RandomSeed { get; set; }

    public Team CurrentTeam { get; set; } = null!;
    public Team NextTeam { get; set; } = null!;
    public User User01 { get; set; } = null!;
    public User User02 { get; set; } = null!;

    public void Play(int userID, int unitInActionID, int baseSkillID, HashSet<int> targetIDs)
    {
        if (userID != CurrentTeam.UserID)
        {
            return;
        }

        var currentUnit = CurrentTeam.Units.SingleOrDefault(x => x.BaseUnitID == unitInActionID);

        if (currentUnit == null)
        {
            return;
        }

        var skill = currentUnit.Skills.SingleOrDefault(x => x.BaseSkillID == baseSkillID);

        if (skill == null)
        {
            return;
        }

        if (!currentUnit.CheckSkillCost(skill))
        {
            return;
        }

        if (!currentUnit.CheckSkillTargets(skill, targetIDs, CurrentTeam.Units, NextTeam.Units))
        {
            return;
        }
        
        currentUnit.ApplySkillCost(skill);
        currentUnit.UseSkill(skill, targetIDs, CurrentTeam.Units, NextTeam.Units, RandomSeed);
    }
}
