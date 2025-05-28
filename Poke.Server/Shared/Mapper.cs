using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Match.Models;

namespace Poke.Server.Shared;

public class Mapper
{
    public static Dictionary<int, Unit> ToMatchTeam(IQueryable<Data.Player.Models.Unit> units)
    {
        return units.Select(x => new Unit
        {
            UnitID = x.UnitID,
            Name = x.UnitName,
            FlatProperties = x.Properties.Select(xp => new FlatProperty
            {
                Name = xp.PropertyName,
                CurrentValue = xp.CurrentValue,
                BaseValue = xp.BaseValue,
            }).ToDictionary(x => x.Name, x => x),
            Skills = x.Skills.ToDictionary(s => s.SkillName, s => new Skill
            {
                SkillID = s.SkillID,
                Name = s.SkillName,
                Behaviors = s.Behaviors.Select(b => new Behavior
                {
                    Name = b.BehaviorName,
                    MinMaxProperties = b.MinMaxProperties.Select(mp => new MinMaxProperty
                    {
                        Name = mp.PropertyName,
                        MinCurrentValue = mp.MinCurrentValue,
                        MaxCurrentValue = mp.MaxCurrentValue,
                        MinBaseValue = mp.MinBaseValue,
                        MaxBaseValue = mp.MaxBaseValue,
                    }).ToList(),
                    FlatProperties = b.Properties.Select(mp => new FlatProperty
                    {
                        Name = mp.PropertyName,
                        CurrentValue = mp.CurrentValue,
                        BaseValue = mp.BaseValue,
                    }).ToList(),
                    Target = new Target
                    {
                        PropertyName = b.Target.TargetPropertyName,
                        Direction = b.Target.TargetDirection,
                        Type = b.Target.TargetType,
                        Quantity = b.Target.Quantity
                    },
                    Costs = b.Costs.Select(c => new Cost
                    {
                        CostType = c.CostType,
                        CostPropertyName = c.PropertyName,
                        CurrentValue = c.FlatProperty.CurrentValue,
                        BaseValue = c.FlatProperty.BaseValue
                    }).ToList()
                }).ToList()
            }),
        })
        .AsNoTracking()
        .AsSplitQuery()
        .ToDictionary(u => u.UnitID);
    }
}
