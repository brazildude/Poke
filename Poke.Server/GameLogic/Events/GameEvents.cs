using MemoryPack;
using Poke.Server.Shared.Enums;

namespace Poke.Server.GameLogic.Events;

[MemoryPackable]
[MemoryPackUnion(0, typeof(UnitStateChangedEvent))]
[MemoryPackUnion(1, typeof(NoResourcesEvent))]
public abstract partial class GameEvent
{
    public long EventId { get; init; } = DateTime.UtcNow.Ticks;
    public string Type { get; init; } = null!;
}

[MemoryPackable]
public partial class UnitStateChangedEvent : GameEvent
{
    public int UnitID { get; set; }
    public string PropertyName { get; set; } = null!;
    public int AppliedValue { get; init; }
    public int CurrentValue { get; init; }
    public HitType HitType { get; set; }
}

[MemoryPackable]
public partial class NoResourcesEvent : GameEvent
{
    public string BehaviorName { get; set; } = null!;
    public string PropertyName { get; set; } = null!;
    public int RequiredValue { get; init; }
    public int CurrentValue { get; init; }
}
