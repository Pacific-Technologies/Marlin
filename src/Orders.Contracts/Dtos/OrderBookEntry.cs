using Binance.Net.Enums;

namespace Marlin.Orders.Contracts.Dtos;

[MemoryPackable]
public readonly partial record struct OrderBookEntry(
    decimal Price,
    decimal Quantity);

[MemoryPackable]
public partial record Quote(
    string Symbol,
    long UpdateId,
    decimal Price,
    decimal Quantity,
    decimal Weight,
    OrderSide Direction)
{
    public decimal WeightedPrice => Price * Weight;
}

[MemoryPackable]
public partial record MarketDepthPair(
    string Symbol,
    long UpdateId,
    Quote Ask,
    Quote Bid)
{
    public decimal PriceSpread => Bid is not null && Ask is not null
        ? Ask!.Price - Bid!.Price
        : default;
    public decimal VolumeSpread => Bid is not null && Ask is not null
        ? Math.Abs(Ask!.Quantity - Bid!.Quantity)
        : default;
    public decimal MediumPrice => Bid is not null && Ask is not null
        ? (Ask!.Price + Bid!.Price) / 2
        : default;
    public decimal MediumWeight => Bid is not null && Ask is not null
        ? (Ask!.Weight + Bid!.Weight) / 2
        : default;
    public decimal WeightedPriceSpread => Bid is not null && Ask is not null
        ? Ask!.WeightedPrice - Bid!.WeightedPrice
        : default;
    public decimal AskRank => Ask is not null
        ? Ask!.Quantity * Ask!.Weight
        : default;
    public decimal BidRank => Bid is not null
        ? Bid!.Quantity * Bid!.Weight / MediumPrice * Bid!.Weight
        : default;
    public decimal Rank => Bid is not null && Ask is not null
        ? BidRank + AskRank
        : default;
}