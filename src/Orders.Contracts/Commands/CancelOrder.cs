namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
public partial record CancelOrder() : OrderCommandBase;