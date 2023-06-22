namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
public partial record CloseOrderBook(
    string OrderBookId,
    long UpdateId,
    DateTime BinanceTimestamp) : OrderBookCommandBase(OrderBookId, UpdateId);