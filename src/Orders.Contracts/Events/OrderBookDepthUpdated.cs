using Marlin.Orders.Contracts.Dtos;

namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
public partial record OrderBookDepthUpdated(
    string OrderBookId,
    long FirstUpdateId,
    long LastUpdateId,
    OrderBookEntry[] Asks,
    OrderBookEntry[] Bids,
    DateTime BinanceTimestamp) : OrderBookEventBase(OrderBookId);