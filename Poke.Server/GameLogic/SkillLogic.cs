using Poke.Server.Data.Match.Models;
using Poke.Server.Shared.Enums;

namespace Poke.Server.GameLogic;

public class SkillLogic
{
    private static readonly Dictionary<SkillName, Func<Skill, bool>> isInCooldownFuncs = [];

    static SkillLogic()
    {
        // add custom code if any
    }

    public static Func<Skill, bool> IsInCooldown = (skill) =>
    {
        if (isInCooldownFuncs.TryGetValue(skill.Name, out var custom))
        {
            return custom(skill);
        }

        return skill.Behaviors
            .SelectMany(x => x.FlatProperties)
            .Where(x => x.Name == PropertyName.Cooldown)
            .Any(x => x.CurrentValue > 0);
    };

    public static Action<Unit, SkillName> TickCooldown = (aliveUnit, skillName) =>
    {
        var cooldowns = aliveUnit.Skills
            .Where(x => x.Key != skillName)
            .SelectMany(x => x.Value.Behaviors)
            .SelectMany(x => x.FlatProperties
                .Where(p => p.Name == PropertyName.Cooldown && p.CurrentValue > 0)
            );

        foreach (var cooldown in cooldowns)
        {
            cooldown.CurrentValue -= 1;
        }
    };

    public static Func<Skill, Dictionary<int, Unit>, Dictionary<int, Unit>, HashSet<int>, bool> AreTargetsValid =
    (skill, ownUnits, enemyUnits, targetIDs) =>
    {
        foreach (var behavior in skill.Behaviors)
        {
            var targetType = behavior.Target.Type;
            var targetDirection = behavior.Target.Direction;

            // Always valid for Random and Self target types
            if (targetType == TargetType.Random || targetType == TargetType.Self || targetType == TargetType.All)
                break;

            // Cannot be valid if no targets are selected
            if (targetIDs.Count == 0)
                return false;

            var ownUnitIds = ownUnits.Where(u => UnitLogic.IsAlive(u.Value)).Select(u => u.Key).ToHashSet();
            var enemyUnitIds = enemyUnits.Where(u => UnitLogic.IsAlive(u.Value)).Select(u => u.Key).ToHashSet();
            var allUnitIds = ownUnitIds.Union(enemyUnitIds);

            // Ensure all selected targets are valid units
            if (!targetIDs.All(id => allUnitIds.Contains(id)))
                return false;

            // Check for Select target constraints
            if (targetType == TargetType.Select)
            {
                if (targetIDs.Count > behavior.Target.Quantity)
                    return false;
                if ((targetDirection == TargetDirection.Own && targetIDs.Any(enemyUnitIds.Contains)) ||
                    (targetDirection == TargetDirection.Enemy && targetIDs.Any(ownUnitIds.Contains)))
                {
                    return false;
                }
            }
        }

        return true;
    };
}
