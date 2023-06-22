#pragma warning disable IDE0058
using Marlin.Orders.Aggregates;
using Marlin.Orders.Contracts.Events;
using Marten;
using Marten.Events;
using Marten.Services.Json;

namespace Marlin.Orders.Persistence;

public sealed class OrdersMartenConfiguration : IConfigureMarten
{
    public void Configure(IServiceProvider services, StoreOptions options)
    {
        options.Events
            .AddEventType<OrderBookOpened>()
            .AddEventType<OrderBookClosed>()
            .AddEventType<OrderBookDepthUpdated>()
            .AddEventType<WeightConstantUpdated>()
            .AddEventType<BestAskUpdated>()
            .AddEventType<BestBidUpdated>()
            .AddEventType<BestPairUpdated>();

        options.Events.MetadataConfig.EnableAll();
        options.Events.DatabaseSchemaName = "orders";
        options.DatabaseSchemaName = "orders";
        options.Events.StreamIdentity = StreamIdentity.AsString;
        options.Projections.LiveStreamAggregation<OrderBook>();
        options.UseDefaultSerialization(serializerType: SerializerType.SystemTextJson);
    }
}