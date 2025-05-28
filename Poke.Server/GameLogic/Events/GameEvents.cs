namespace Poke.Server.GameLogic.Events;

public abstract class GameEvent
{
    public int EventId { get; set; }
    public string Type { get; init; } = null!;  // e.g. "Damage", "Dodge", "Crit"
}

public class CostEvent : GameEvent
{
    public int UnitID { get; init; }
    public string CostPropertyName { get; init; } = null!;
    public int CostValue { get; init; }
}


public class DamageEvent : GameEvent
{
    public int SourceUnitId { get; init; }
    public int TargetUnitId { get; init; }
    public string PropertyName { get; init; } = null!;
    public int Amount { get; init; }
    public bool IsCritical { get; init; }
}

public class DodgeEvent : GameEvent
{
    public int AttackerUnitId { get; init; }
    public int DefenderUnitId { get; init; }
}

public class HealEvent : GameEvent
{
    public int HealerUnitId { get; init; }
    public int TargetUnitId { get; init; }
    public int Amount { get; init; }
}
