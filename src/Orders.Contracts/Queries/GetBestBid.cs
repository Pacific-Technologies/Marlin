namespace Marlin.Orders.Contracts.Queries;

[MemoryPackable]
[WolverineMessage]
public partial record GetBestBid(string OrderBookId, long UpdateId);
