using Poke.Server.Data.Enums;

namespace Poke.Server.Data.Models;

public class Target
{
    public int TargetID { get; set; }
    public TargetType TargetType { get; set; }
    public TargetDirection TargetDirection { get; set; }
    public int Quantity { get; set; }

    public List<Unit> GetTargets(Unit unitInAction, List<Unit> ownUnits, List<Unit> enemyUnits)
    {
        if (TargetType == TargetType.Self)
        {
            return new List<Unit> { unitInAction };
        }

        if (TargetType == TargetType.Select)
        {
            if (TargetDirection == TargetDirection.Enemy)
            {

            }
        }

        return new List<Unit> { unitInAction };
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
