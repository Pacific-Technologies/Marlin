namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
public partial record UpdateBestAsk(
    string OrderBookId,
    long UpdateId,
    decimal Price,
    decimal Quantity) : OrderBookCommandBase(OrderBookId, UpdateId);