#pragma warning disable IDE0058
using Marlin.Orders.Persistence;
using Marlin.Orders.Messaging;
using Oakton;
using Marlin.Orders.Services;
using Marlin.Common.Binance;
using Marlin.Common.Logging;

var host = Host
    .CreateDefaultBuilder(args)
    .ApplyOaktonExtensions()
    .ConfigureLogging(c => c.ClearProviders())
    .ConfigureServices((host, services) =>
    {
        services
            .AddHostedService<MarketDepthService>()
            .AddPersitence(host.Configuration)
            .AddOptions<BinanceSettings>()
            .BindConfiguration(nameof(BinanceSettings));
    })
    .AddLogging()
    .AddMessaging(default!)
    .Build();

host.RunOaktonCommandsSynchronously(args);