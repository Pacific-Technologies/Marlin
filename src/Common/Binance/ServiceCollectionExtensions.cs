#pragma warning disable IDE0058
using Microsoft.Extensions.DependencyInjection;
using Binance.Net;
using Binance.Net.Objects;

namespace Marlin.Common.Binance;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBinanceClients(
        this IServiceCollection services,
        BinanceSettings settings)
    {
        return services.AddBinance((restOpts, socketOpts) =>
        {
            restOpts.ApiCredentials = new BinanceApiCredentials(settings.Key, settings.Secret);
            restOpts.SpotApiOptions.BaseAddress = settings.BaseAddress;
            socketOpts.ApiCredentials = new BinanceApiCredentials(settings.Key, settings.Secret);
            socketOpts.SpotApiOptions.BaseAddress = settings.BaseAddress;
        });
    }
}