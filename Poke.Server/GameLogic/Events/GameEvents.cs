using MemoryPack;

namespace Poke.Server.GameLogic.Events;

[MemoryPackable]
[MemoryPackUnion(0, typeof(CostEvent))]
[MemoryPackUnion(1, typeof(DamageEvent))]
[MemoryPackUnion(2, typeof(DodgeEvent))]
[MemoryPackUnion(3, typeof(HealEvent))]
public abstract partial class GameEvent
{
    public long EventId { get; init; } = DateTime.UtcNow.Ticks;
    public string Type { get; init; } = null!;  // e.g. "Damage", "Dodge", "Crit"
}

[MemoryPackable]
public partial class UnitSelectedEvent : GameEvent
{
    public int UnitID { get; init; }
}

[MemoryPackable]
public partial class SkillSelectedEvent : GameEvent
{
    public int SkillID { get; init; }
}

[MemoryPackable]
public partial class CostEvent : GameEvent
{
    public int UnitID { get; init; }
    public string CostPropertyName { get; init; } = null!;
    public int CostValue { get; init; }
}

[MemoryPackable]
public partial class DamageEvent : GameEvent
{
    public int SourceUnitId { get; init; }
    public int TargetUnitId { get; init; }
    public string PropertyName { get; init; } = null!;
    public int Amount { get; init; }
    public bool IsCritical { get; init; }
}

[MemoryPackable]
public partial class DodgeEvent : GameEvent
{
    public int AttackerUnitId { get; init; }
    public int DefenderUnitId { get; init; }
}

[MemoryPackable]
public partial class HealEvent : GameEvent
{
    public int HealerUnitId { get; init; }
    public int TargetUnitId { get; init; }
    public int Amount { get; init; }
}
