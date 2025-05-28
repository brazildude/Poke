using Microsoft.EntityFrameworkCore;
using Poke.Server.Data.Match.Models;

namespace Poke.Server.Shared.Mappers;

public class PlayerMapper
{
    public static Dictionary<int, Unit> ToMatchTeam(IQueryable<Data.Player.Models.Unit> units)
    {
        return units.Select(x => new Unit
        {
            UnitID = x.UnitID,
            Name = x.Name,
            FlatProperties = x.FlatProperties.Select(xp => new FlatProperty
            {
                Name = xp.Name,
                CurrentValue = xp.CurrentValue,
                BaseValue = xp.BaseValue,
            }).ToDictionary(x => x.Name, x => x),
            Skills = x.Skills.ToDictionary(s => s.Name, s => new Skill
            {
                SkillID = s.SkillID,
                Name = s.Name,
                Behaviors = s.Behaviors.Select(b => new Behavior
                {
                    Name = b.Name,
                    MinMaxProperties = b.MinMaxProperties.Select(mp => new MinMaxProperty
                    {
                        Name = mp.Name,
                        MinCurrentValue = mp.MinCurrentValue,
                        MaxCurrentValue = mp.MaxCurrentValue,
                        MinBaseValue = mp.MinBaseValue,
                        MaxBaseValue = mp.MaxBaseValue,
                    }).ToList(),
                    FlatProperties = b.FlatProperties.Select(mp => new FlatProperty
                    {
                        Name = mp.Name,
                        CurrentValue = mp.CurrentValue,
                        BaseValue = mp.BaseValue,
                    }).ToList(),
                    Target = new Target
                    {
                        TargetPropertyName = b.Target.TargetPropertyName,
                        Direction = b.Target.Direction,
                        Type = b.Target.Type,
                        Quantity = b.Target.Quantity
                    },
                    Costs = b.Costs.Select(c => new Cost
                    {
                        CostType = c.Type,
                        CostPropertyName = c.CostPropertyName,
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
