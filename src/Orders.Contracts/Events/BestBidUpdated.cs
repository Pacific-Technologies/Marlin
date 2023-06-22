using Marlin.Orders.Contracts.Dtos;

namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
public partial record BestBidUpdated(
    string OrderBookId,
    Quote Quote) : OrderBookEventBase(OrderBookId);