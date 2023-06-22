using Binance.Net.Enums;
using Marlin.Orders.Contracts.Dtos;
using Marlin.Orders.Contracts.Events;
using Marlin.Orders.Util;

namespace Marlin.Orders.Aggregates;

public sealed class OrderBook
{
    public string Id { get; private set; }
    public string Symbol { get; private set; }
    public long Version { get; internal set; }
    public long LastUpdateId { get; private set; }
    public bool Fresh { get; private set; } = true;

    public DateTime BinanceTimestamp { get; private set; }
    public DateTime BinanceOpenTime { get; private set; }
    public DateTime? BinanceCloseTime { get; private set; }
    public DateTime LastUpdate { get; private set; }
    public DateTime OpenedAt { get; private set; }
    public DateTime? ClosedOn { get; private set; }

    private readonly SortedDictionary<decimal, decimal> _asks = new();
    private readonly SortedDictionary<decimal, decimal> _bids = new(new DescendingDecimalComparer());
    private readonly SortedDictionary<decimal, int> _askHourMap = new();
    private readonly SortedDictionary<decimal, decimal> _askWeightMap = new();
    private readonly SortedDictionary<decimal, int> _bidHourMap = new();
    private readonly SortedDictionary<decimal, decimal> _bidWeightMap = new();

    public IEnumerable<OrderBookEntry> Bids =>
        _bids.Select(x =>
            new OrderBookEntry(x.Key, x.Value)
        );

    public IEnumerable<OrderBookEntry> Asks =>
        _asks.Select(x =>
            new OrderBookEntry(x.Key, x.Value)
        );

    public IEnumerable<Quote> BidQuotes =>
        Bids.Select(x =>
            new Quote(
                Symbol,
                LastUpdateId,
                x.Price,
                x.Quantity,
                _bidWeightMap[x.Price],
                OrderSide.Buy
            )
        );

    public IEnumerable<Quote> AskQuotes =>
        Asks.Select(x =>
            new Quote(
                Symbol,
                LastUpdateId,
                x.Price,
                x.Quantity,
                _askWeightMap[x.Price],
                OrderSide.Buy
            )
        );

    public decimal WeightConstant { get; set; }
    public Quote? BestBid { get; private set; }
    public Quote? BestAsk { get; private set; }
    public MarketDepthPair? BestPair { get; private set; }

    public OrderBook(OrderBookOpened @event)
    {
        Id = @event.OrderBookId;
        Symbol = @event.Symbol;
        LastUpdate = @event.TriggeredOn;
        LastUpdateId = @event.UpdateId;
        BinanceOpenTime = @event.BinanceTimestamp;
        BinanceTimestamp = @event.BinanceTimestamp;
        WeightConstant = @event.WeightConstant;
        foreach (var bid in @event.Bids)
        {
            if (bid.Quantity < 1e-11M)
            {
                continue;
            }
            _bids[bid.Price] = bid.Quantity;
            _askHourMap[bid.Price] = BinanceTimestamp.Hour;
        }
        foreach (var ask in @event.Asks)
        {
            if (ask.Quantity < 1e-11M)
            {
                continue;
            }
            _asks[ask.Price] = ask.Quantity;
            _askHourMap[ask.Price] = BinanceTimestamp.Hour;
        }
    }

    public void Apply(WeightConstantUpdated @event)
    {
        WeightConstant = @event.WeightConstant;
    }

    public void Apply(OrderBookDepthUpdated @event)
    {
        LastUpdate = @event.TriggeredOn;
        LastUpdateId = @event.LastUpdateId;
        BinanceTimestamp = @event.BinanceTimestamp;
        Fresh = false;
        foreach (var bid in @event.Bids)
        {
            if (bid.Quantity < 1e-11M)
            {
                _ = _bids.Remove(bid.Price);
                continue;
            }
            _bids[bid.Price] = bid.Quantity;
            if (!_bidHourMap.TryGetValue(bid.Price, out int lastSeen))
            {
                lastSeen = BinanceTimestamp.Hour;
            }
            _bidWeightMap[bid.Price]
                = 1 / ((WeightConstant * (BinanceTimestamp.Hour - lastSeen)) + 1);
            _bidHourMap[bid.Price] = BinanceTimestamp.Hour;
        }
        foreach (var ask in @event.Asks)
        {
            if (ask.Quantity < 1e-11M)
            {
                _ = _asks.Remove(ask.Price);
                continue;
            }
            if (!_askHourMap.TryGetValue(ask.Price, out int lastSeen))
            {
                lastSeen = BinanceTimestamp.Hour;
            }
            _askWeightMap[ask.Price]
                = 1 / ((WeightConstant * (BinanceTimestamp.Hour - lastSeen)) + 1);
            _askHourMap[ask.Price] = BinanceTimestamp.Hour;
        }
    }

    public void Apply(BestAskUpdated @event)
    {
        // LastUpdate = @event.TriggeredOn;
        // LastUpdateId = @event.UpdateId;
        // BinanceTimestamp = @event.BinanceTimestamp;
        BestAsk = @event.Quote;
    }

    public void Apply(BestBidUpdated @event)
    {
        // LastUpdate = @event.TriggeredOn;
        // LastUpdateId = @event.UpdateId;
        // BinanceTimestamp = @event.BinanceTimestamp;
        BestBid = @event.Quote;
    }

    public void Apply(BestPairUpdated @event)
    {
        // LastUpdate = @event.TriggeredOn;
        // LastUpdateId = @event.UpdateId;
        // BinanceTimestamp = @event.BinanceTimestamp;
        BestPair = @event.Pair;
    }

    public void Apply(OrderBookClosed @event)
    {
        LastUpdate = @event.TriggeredOn;
        LastUpdateId = @event.UpdateId;
        ClosedOn = @event.TriggeredOn;
        BinanceTimestamp = @event.BinanceTimestamp;
        BinanceCloseTime = @event.BinanceTimestamp;
    }
}