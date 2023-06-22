using Marlin.Orders.Contracts.Dtos;

namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
public partial record BestPairUpdated(
    string OrderBookId,
    MarketDepthPair Pair) : OrderBookEventBase(OrderBookId);