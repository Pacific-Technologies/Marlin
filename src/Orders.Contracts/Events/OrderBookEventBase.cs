namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
[MemoryPackUnion(0, typeof(OrderBookOpened))]
[MemoryPackUnion(1, typeof(OrderBookDepthUpdated))]
[MemoryPackUnion(2, typeof(WeightConstantUpdated))]
[MemoryPackUnion(3, typeof(BestAskUpdated))]
[MemoryPackUnion(4, typeof(BestBidUpdated))]
[MemoryPackUnion(5, typeof(BestPairUpdated))]
[MemoryPackUnion(6, typeof(OrderBookClosed))]
public abstract partial record OrderBookEventBase(string OrderBookId)
{
    public DateTime TriggeredOn { get; } = DateTime.UtcNow;
}