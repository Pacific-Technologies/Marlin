namespace Marlin.Orders.Contracts.Queries;

[MemoryPackable]
[WolverineMessage]
public partial record GetVersionUpdateIdMap(string OrderBookId);