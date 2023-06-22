namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
[MemoryPackUnion(0, typeof(OpenOrderBook))]
[MemoryPackUnion(1, typeof(UpdateOrderBookDepth))]
[MemoryPackUnion(2, typeof(UpdateWeightConstant))]
[MemoryPackUnion(3, typeof(UpdateBestAsk))]
[MemoryPackUnion(4, typeof(UpdateBestBid))]
[MemoryPackUnion(5, typeof(UpdateBestPair))]
[MemoryPackUnion(6, typeof(CloseOrderBook))]
public abstract partial record OrderBookCommandBase(string OrderBookId, long UpdateId);