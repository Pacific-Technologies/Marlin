using Marten;
using Wolverine.Marten;

namespace Marlin.Orders.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersitence(this IServiceCollection services, IConfiguration config)
    {
        return services
            .AddSingleton<IConfigureMarten, OrdersMartenConfiguration>()
            .AddMarten(config.GetConnectionString("marten")!)
            .ApplyAllDatabaseChangesOnStartup()
            .UseLightweightSessions()
            .OptimizeArtifactWorkflow()
            .IntegrateWithWolverine("orders")
            .EventForwardingToWolverine()
            .Services;
    }
}