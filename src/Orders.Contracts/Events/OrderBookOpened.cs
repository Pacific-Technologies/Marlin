using Marlin.Orders.Contracts.Dtos;

namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
public partial record OrderBookOpened(
    string OrderBookId,
    string Symbol,
    long UpdateId,
    decimal WeightConstant,
    OrderBookEntry[] Asks,
    OrderBookEntry[] Bids,
    DateTime BinanceTimestamp) : OrderBookEventBase(OrderBookId);