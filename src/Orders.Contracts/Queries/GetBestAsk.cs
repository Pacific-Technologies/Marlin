namespace Marlin.Orders.Contracts.Queries;

[MemoryPackable]
[WolverineMessage]
public partial record GetBestAsk(string OrderBookId, long UpdateId);