namespace Marlin.Orders.Contracts.Events;

[MemoryPackable]
public partial record WeightConstantUpdated(
    string OrderBookId,
    decimal WeightConstant) : OrderBookEventBase(OrderBookId);