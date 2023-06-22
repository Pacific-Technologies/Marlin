namespace Marlin.Orders.Contracts.Commands;

[MemoryPackable]
public partial record UpdateWeightConstant(
    string OrderBookId,
    long UpdateId,
    decimal WeightConstant) : OrderBookCommandBase(OrderBookId, UpdateId);