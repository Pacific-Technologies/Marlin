using Marlin.Orders.Contracts.Dtos;

namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
public partial record OpenOrderBook(
    string OrderBookId,
    string Symbol,
    long UpdateId,
    decimal WeightConstant,
    OrderBookEntry[] Asks,
    OrderBookEntry[] Bids,
    DateTime BinanceTimestamp) : OrderBookCommandBase(OrderBookId, UpdateId);