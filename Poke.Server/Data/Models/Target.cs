using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public class Target
{
    public int TargetID { get; set; }
    public TargetType TargetType { get; set; }
    public TargetDirection TargetDirection { get; set; }
    public int Quantity { get; set; }

    public List<BaseUnit> GetTargets(BaseUnit unitInAction, List<BaseUnit> ownUnits, List<BaseUnit> enemyUnits)
    {
        if (TargetType == TargetType.Self)
        {
            return new List<BaseUnit> { unitInAction };
        }

        if (TargetType == TargetType.Single)
        {
            if (TargetDirection == TargetDirection.Enemy)
            {

            }
        }

        return new List<BaseUnit> { unitInAction };
    }

    public static Target New(TargetType targetType, TargetDirection targetDirection, int quantity)
    {
        return new Target
        {
            TargetType = targetType,
            TargetDirection = targetDirection,
            Quantity = quantity
        };
    }
}
