using Marlin.Orders.Contracts.Dtos;

namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
public partial record UpdateOrderBookDepth(
    string OrderBookId,
    long FirstUpdateId,
    long LastUpdateId,
    OrderBookEntry[] Asks,
    OrderBookEntry[] Bids,
    DateTime BinanceTimestamp) : OrderBookCommandBase(OrderBookId, LastUpdateId);