using System.Threading.Channels;
using Binance.Net.Clients;
using Binance.Net.Interfaces;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using Marlin.Common.Binance;
using Marlin.Orders.Aggregates;
using Marlin.Orders.Contracts.Commands;
using Marlin.Orders.Contracts.Dtos;
using Marten;
using Microsoft.Extensions.Options;
using Wolverine;

namespace Marlin.Orders.Services;

public sealed class MarketDepthService : IHostedService, IDisposable
{
    private readonly ILogger<MarketDepthService> _logger;
    private readonly CancellationTokenSource _cancellation;
    private Task? _task;
    private readonly ChannelReader<UpdateOrderBookDepth> _reader;
    private readonly ChannelWriter<UpdateOrderBookDepth> _writer;
    private readonly BinanceSocketClient _socketClient;
    private readonly BinanceClient _client;
    private readonly BinanceSettings _settings;
    private readonly IMessageBus _bus;
    private readonly IDocumentSession _session;

    public MarketDepthService(
        IMessageBus bus,
        IDocumentSession session,
        IOptions<BinanceSettings> settings,
        ILogger<MarketDepthService> logger)
    {
        _bus = bus;
        _session = session;
        _settings = settings.Value;
        _cancellation = new CancellationTokenSource();
        _socketClient = new BinanceSocketClient();
        _client = new BinanceClient();
        _logger = logger;
        var channelOpts = new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true
        };
        var channel = Channel.CreateBounded<UpdateOrderBookDepth>(channelOpts);
        _reader = channel.Reader;
        _writer = channel.Writer;
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        bool subbed = false;
        while (!subbed)
        {
            var result = await _socketClient.SpotApi.ExchangeData.SubscribeToOrderBookUpdatesAsync(
                _settings.Symbols,
                1000,
                async (e) =>
                {
                    try
                    {
                        await _writer.WriteAsync(MapToCommand(e.Data), cancellationToken)
                            .ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing market depth update");
                    }
                },
                cancellationToken
            ).ConfigureAwait(false);
            // Log any errors and wait two seconds before trying again
            if (!result.Success)
            {
                _logger.LogError(
                    "Error {Code}: {Message} with data {Data}",
                    result.Error!.Data,
                    result.Error.Message,
                    result.Error.Data);
                await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
                continue;
            }
            subbed = true;
        }
        // Fetch the snapshots
        foreach (string symbol in _settings.Symbols)
        {
            // string id = $"{symbol}_{DateTime.UtcNow.Date}";
            // var book = await _session.Events.AggregateStreamAsync<OrderBook>(
            //     id,
            //     token: cancellationToken).ConfigureAwait(false);
            // if (book is not null)
            // {
            //     _session.Events.ArchiveStream(id);
            //     await _session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            // }
            var response = await _client.SpotApi.ExchangeData.GetOrderBookAsync(
                symbol,
                100,
                cancellationToken
            ).ConfigureAwait(false);
            // Open the order book
            // _logger.LogInformation(
            //     "# {Symbol}: Opening order book at {UpdateId}",
            //     response.Data.Symbol,
            //     response.Data.LastUpdateId);
            await _bus.SendAsync(MapToCommand(response.Data, DateTime.UtcNow)).ConfigureAwait(false);
        }
        // Process and send the UpdateOrderBookDepth commands as they arrive
        await foreach (var command in _reader.ReadAllAsync(cancellationToken))
        {
            // _logger.LogInformation(
            //     "# Updating order book {Id} from {First} to {Last}",
            //     command.OrderBookId,
            //     command.FirstUpdateId,
            //     command.LastUpdateId);
            await _bus.SendAsync(command).ConfigureAwait(false);
        }
    }

    private static OpenOrderBook MapToCommand(BinanceOrderBook book, DateTime timestamp)
    {
        var bids = book.Bids.Select(e => new OrderBookEntry(e.Price, e.Quantity)).ToArray();
        var asks = book.Asks.Select(e => new OrderBookEntry(e.Price, e.Quantity)).ToArray();
        return new OpenOrderBook(
            $"{book.Symbol}_{timestamp.Date}",
            book.Symbol,
            book.LastUpdateId,
            1M / 6M,
            asks,
            bids,
            timestamp);
    }

    private static UpdateOrderBookDepth MapToCommand(IBinanceEventOrderBook data)
    {
        var bids = data.Bids.Select(e => new OrderBookEntry(e.Price, e.Quantity)).ToArray();
        var asks = data.Asks.Select(e => new OrderBookEntry(e.Price, e.Quantity)).ToArray();
        return new UpdateOrderBookDepth(
            $"{data.Symbol}_{data.EventTime.Date}",
            data.FirstUpdateId ?? data.LastUpdateId,
            data.LastUpdateId,
            asks,
            bids,
            data.EventTime);
    }

    public void Dispose()
    {
        _task?.Dispose();
        _client.Dispose();
        _socketClient.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _socketClient.SetApiCredentials(new BinanceApiCredentials(_settings.Key, _settings.Secret));
        _client.SetApiCredentials(new BinanceApiCredentials(_settings.Key, _settings.Secret));
        _task = ExecuteAsync(_cancellation.Token);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _ = _writer.TryComplete();
        _cancellation.Cancel();
        await _task!.ConfigureAwait(false);
        Dispose();
    }
}