namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
public partial record OrderBookClosed(
    string OrderBookId,
    long UpdateId,
    DateTime BinanceTimestamp) : OrderBookEventBase(OrderBookId);