using Marlin.Orders.Aggregates;
using Marlin.Orders.Contracts.Commands;
using Marlin.Orders.Services;
using Wolverine;
using Wolverine.Marten;

namespace Marlin.Orders.Commands;

public static class UpdateOrderBookDepthAggregateHandler
{
    public static (Events, OutgoingMessages) Handle(
        UpdateOrderBookDepth command,
        OrderBook state,
        ILogger logger)
    {
        try
        {
            logger.LogInformation(
                "Updating order book {Id} from {First} to {Last}",
                command.OrderBookId,
                command.FirstUpdateId,
                command.LastUpdateId);
            var events = new Events();
            var messages = new OutgoingMessages();
            events += OrderBookService.Execute(command, state);
            var bestBid = OrderBookService.GetBestBid(state);
            var bestAsk = OrderBookService.GetBestAsk(state);
            messages.Add(
                new UpdateBestAsk(
                    command.OrderBookId,
                    command.LastUpdateId,
                    bestAsk!.Price,
                    bestAsk.Quantity));
            messages.Add(
                new UpdateBestBid(
                    command.OrderBookId,
                    command.LastUpdateId,
                    bestBid!.Price,
                    bestBid.Quantity));
            messages.Add(
                new UpdateBestPair(
                    command.OrderBookId,
                    command.LastUpdateId,
                    bestAsk,
                    bestBid));
            return (events, messages);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error executing command: {nameof(UpdateOrderBookDepth)}");
            throw;
        }
    }
}