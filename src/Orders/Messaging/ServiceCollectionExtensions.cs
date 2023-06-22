#pragma warning disable IDE0058
using Wolverine;

namespace Marlin.Orders.Messaging;

public static class ServiceCollectionExtensions
{
    public static IHostBuilder AddMessaging(this IHostBuilder builder, IConfiguration config)
    {
        return builder.UseWolverine(opts =>
        {
            opts.Include<OrdersExtension>(e =>
            {
                e.SetRabbitMQSettings(config);
                e.Configure(opts);
            });
        });
    }
}
