using Marlin.Orders.Aggregates;
using Marlin.Orders.Contracts.Commands;
using Marlin.Orders.Contracts.Dtos;
using Marlin.Orders.Contracts.Events;

namespace Marlin.Orders.Services;

public static class OrderBookService
{
    public static OrderBookOpened Create(OpenOrderBook command)
    {
        if (command.OrderBookId != $"{command.Symbol}_{command.BinanceTimestamp.Date}")
        {
            throw new ArgumentException("Invalid OrderBookId", nameof(command));
        }
        if (string.IsNullOrWhiteSpace(command.Symbol))
        {
            throw new ArgumentException("Invalid Symbol", nameof(command));
        }
        return command.UpdateId == 0
            ? throw new ArgumentException("Invalid UpdateId", nameof(command))
            : new OrderBookOpened(
                command.OrderBookId,
                command.Symbol,
                command.UpdateId,
                command.WeightConstant,
                command.Asks,
                command.Bids,
                command.BinanceTimestamp
            );
    }

    public static WeightConstantUpdated Execute(
        UpdateWeightConstant command,
        OrderBook state)
    {
        if (command.OrderBookId != state.Id)
        {
            throw new ArgumentException("Invalid OrderBookId", nameof(command));
        }
        if (command.UpdateId <= state.LastUpdateId)
        {
            throw new InvalidOperationException(
                $"LastUpdateId {command} less than {state.LastUpdateId}");
        }
        if (command.WeightConstant < 1)
        {
            throw new ArgumentException("WeightConstant must be >= 1", nameof(command));
        }
        var @event = new WeightConstantUpdated(
            command.OrderBookId,
            command.WeightConstant
        );
        state.Apply(@event);
        state.Version++;
        return @event;
    }

    public static OrderBookDepthUpdated Execute(
        UpdateOrderBookDepth command,
        OrderBook state)
    {
        if (command.OrderBookId != state.Id)
        {
            throw new ArgumentException("Invalid OrderBookId", nameof(command));
        }
        if (command.LastUpdateId <= state.LastUpdateId)
        {
            throw new InvalidOperationException(
                $"LastUpdateId {command.LastUpdateId} less than {state.LastUpdateId}");
        }
        if ((command.FirstUpdateId > state.LastUpdateId + 1)
            && state.Fresh)
        {
            throw new InvalidOperationException(
                $"Invalid initial event: {command.FirstUpdateId}/{state.LastUpdateId + 1}");
        }
        if (command.LastUpdateId < state.LastUpdateId + 1 && state.Fresh)
        {
            throw new InvalidOperationException(
                $"Invalid initial event: {command.LastUpdateId}/{state.LastUpdateId + 1}");
        }
        if (command.FirstUpdateId != state.LastUpdateId + 1 && !state.Fresh)
        {
            throw new InvalidOperationException(
                $"Expected FirstUpdateId {state.LastUpdateId + 1} but got {command.FirstUpdateId}"
            );
        }
        var @event = new OrderBookDepthUpdated(
            command.OrderBookId,
            command.FirstUpdateId,
            command.LastUpdateId,
            command.Asks,
            command.Bids,
            command.BinanceTimestamp
        );
        state.Apply(@event);
        state.Version++;
        return @event;
    }

    public static BestAskUpdated Execute(
        UpdateBestAsk command,
        OrderBook state)
    {
        if (command.OrderBookId != state.Id)
        {
            throw new ArgumentException("Invalid OrderBookId", nameof(command));
        }
        if (command.UpdateId != state.LastUpdateId)
        {
            throw new ArgumentException("Invalid UpdateId", nameof(command));
        }
        var quote = state.AskQuotes.FirstOrDefault(q => q.Price == command.Price)
            ?? throw new InvalidOperationException($"Quoute for {command.Price} not found");
        return new BestAskUpdated(
            command.OrderBookId,
            quote
        );
    }

    public static BestBidUpdated Execute(
        UpdateBestBid command,
        OrderBook state)
    {
        if (command.OrderBookId != state.Id)
        {
            throw new ArgumentException("Invalid OrderBookId", nameof(command));
        }
        if (command.UpdateId != state.LastUpdateId)
        {
            throw new ArgumentException("Invalid UpdateId", nameof(command));
        }
        var quote = state.BidQuotes.FirstOrDefault(q => q.Price == command.Price)
            ?? throw new InvalidOperationException($"Quoute for {command.Price} not found");
        var @event = new BestBidUpdated(command.OrderBookId, quote);
        state.Apply(@event);
        state.Version++;
        return @event;
    }

    public static BestPairUpdated Execute(
        UpdateBestPair command,
        OrderBook state)
    {
        if (command.OrderBookId != state.Id)
        {
            throw new ArgumentException("Invalid OrderBookId", nameof(command));
        }
        if (command.UpdateId != state.LastUpdateId)
        {
            throw new ArgumentException("command.UpdateId != state.LastUpdateId", nameof(command));
        }
        var @event = new BestPairUpdated(
            command.OrderBookId,
            new MarketDepthPair(
                state.Symbol,
                command.UpdateId,
                command.Ask,
                command.Bid)
        );
        state.Apply(@event);
        state.Version++;
        return @event;
    }

    public static OrderBookClosed Execute(
        CloseOrderBook command,
        OrderBook state)
    {
        if (command.OrderBookId != state.Id)
        {
            throw new ArgumentException("Invalid OrderBookId", nameof(command));
        }
        if (command.UpdateId != state.LastUpdateId)
        {
            throw new ArgumentException("command.UpdateId != state.LastUpdateId", nameof(command));
        }
        if (state.BinanceCloseTime.HasValue || state.ClosedOn.HasValue)
        {
            throw new InvalidOperationException("OrderBook already closed");
        }
        var @event = new OrderBookClosed(
            command.OrderBookId,
            command.UpdateId,
            command.BinanceTimestamp
        );
        state.Apply(@event);
        state.Version++;
        return @event;
    }

    public static Quote? GetBestBid(OrderBook state)
    {
        return state.BidQuotes.FirstOrDefault();
    }

    public static Quote? GetBestAsk(OrderBook state)
    {
        return state.AskQuotes.FirstOrDefault();
    }

    public static MarketDepthPair? GetBestPair(OrderBook state)
    {
        return state.BestBid is not null && state.BestAsk is not null
            ? new MarketDepthPair(
                state.Symbol,
                state.LastUpdateId,
                state.BestAsk,
                state.BestBid)
            : default;
    }
}