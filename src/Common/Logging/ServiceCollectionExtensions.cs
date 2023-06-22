#pragma warning disable IDE0058
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Marlin.Common.Logging;

public static class ServiceCollectionExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder builder)
    {
        return builder.UseSerilog((host, config) =>
        {
            config.MinimumLevel.Debug();
            config.ReadFrom.Configuration(host.Configuration);
            config.Enrich.FromLogContext().WriteTo.Console(
                LogEventLevel.Debug,
                "[{Timestamp:HH:mm:ss} {Level:u3}]|{SourceContext}| {Message:lj}{NewLine}{Exception}",
                CultureInfo.CurrentCulture);
        });
    }
}