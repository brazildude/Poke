using Poke.Server.Data.Player.Models;
using static Poke.Server.Infrastructure.ViewModels;

namespace Poke.Server.Shared.Mappers;

public class VMMapper
{
    public static IEnumerable<FlatPropertyVM> SelectProperties(List<FlatProperty> x)
    {
        return x.Select(p => new FlatPropertyVM(p.Name.ToString(), p.CurrentValue));
    }

    public static IEnumerable<CostVM> SelectCosts(List<Cost> x)
    {
        return x.Select(c => new CostVM(c.Type.ToString(), c.CostPropertyName.ToString(), c.FlatProperty.CurrentValue));
    }

    public static IEnumerable<SkillVM> SelectSkills(List<Skill> x)
    {
        return x.Select(s => new SkillVM(s.Name.ToString(), SelectProperties(s.FlatProperties), SelectBehaviors(s.Behaviors)));
    }

    public static IEnumerable<MinMaxPropertyVM> SelectMinMaxProperties(List<MinMaxProperty> x)
    {
        return x.Select(c => new MinMaxPropertyVM(c.Name.ToString(), c.MinCurrentValue, c.MaxCurrentValue));
    }

    public static IEnumerable<BehaviorVM> SelectBehaviors(List<Behavior> x)
    {
        return x.Select(b =>
            new BehaviorVM(
                b.Type.ToString(),
                b.Target.TargetPropertyName.ToString(),
                b.Target.Type.ToString(),
                b.Target.Direction.ToString(),
                b.Target.Quantity,
                SelectMinMaxProperties(b.MinMaxProperties),
                SelectCosts(b.Costs)
            )
        );
    }
}
