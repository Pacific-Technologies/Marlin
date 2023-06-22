using Marlin.Orders.Contracts.Dtos;

namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
public partial record UpdateBestPair(
    string OrderBookId,
    long UpdateId,
    Quote Ask,
    Quote Bid) : OrderBookCommandBase(OrderBookId, UpdateId);