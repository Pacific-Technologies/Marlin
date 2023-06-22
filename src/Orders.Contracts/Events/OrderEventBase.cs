namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
[MemoryPackUnion(0, typeof(OrderCreated))]
[MemoryPackUnion(1, typeof(OrderCancelled))]
[MemoryPackUnion(2, typeof(OrderFilled))]
public abstract partial record OrderEventBase()
{
    public DateTime TriggeredOn { get; } = DateTime.UtcNow;
}