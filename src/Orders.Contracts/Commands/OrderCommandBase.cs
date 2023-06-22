namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
[MemoryPackUnion(0, typeof(CreateOrder))]
[MemoryPackUnion(1, typeof(CancelOrder))]
[MemoryPackUnion(2, typeof(FillOrder))]
public abstract partial record OrderCommandBase();