namespace Marlin.Orders.Contracts.Queries;

[MemoryPackable]
[WolverineMessage]
public partial record GetBestPair(string OrderBookId, long UpdateId);