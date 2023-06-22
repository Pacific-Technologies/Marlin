#pragma warning disable IDE0058
using Marlin.Orders.Contracts.Events;
using Marlin.Orders.Contracts.Commands;
using Wolverine;
using Wolverine.MemoryPack;
using Wolverine.RabbitMQ;
using Wolverine.Marten;
using Oakton.Resources;

namespace Marlin.Orders.Messaging;

public class OrdersExtension : IWolverineExtension
{
    private string? _rabbitHost = "localhost";
    private string? _rabbitUser = "rabbit";
    private string? _rabbitPassword = "changeme";

    public void SetRabbitMQSettings(IConfiguration config)
    {
        // _rabbitHost = config.GetValue<string>("RabbitHost");
        // _rabbitUser = config.GetValue<string>("RabbitUser");
        // _rabbitPassword = config.GetValue<string>("RabbitPassword");
    }

    public void Configure(WolverineOptions options)
    {
        options.OptimizeArtifactWorkflow();
        options.Services.AddResourceSetupOnStartup();
        options.Policies.AutoApplyTransactions();
        options.Policies.LogMessageStarting(LogLevel.Debug);
        // options.Policies.UseDurableInboxOnAllListeners();
        // options.Policies.UseDurableOutboxOnAllSendingEndpoints();
        // options.UseMemoryPackSerialization();
        options.Publish(p =>
        {
            p.MessagesImplementing<OrderBookCommandBase>();
            p.ToRabbitQueue("orderbook_commands")
            .UseMemoryPackSerialization()
            .UseDurableOutbox();
        });
        options.Publish(p =>
        {
            p.MessagesImplementing<OrderBookEventBase>();
            p.ToRabbitQueue("orderbook_events")
            .UseMemoryPackSerialization()
            .UseDurableOutbox();
        });
        options.ListenToRabbitQueue("orderbook_commands")
            .Sequential()
            .UseMemoryPackSerialization()
            .UseDurableInbox();
        options
            .UseRabbitMq(conf =>
            {
                conf.HostName = _rabbitHost;
                conf.UserName = _rabbitUser;
                conf.Password = _rabbitPassword;
            })
            .DeclareExchange(
                "orders",
                e =>
                {
                    e.BindQueue("orderbook_events");
                    e.BindQueue("orderbook_commands");
                }
            );
    }
}