using Marlin.Orders.Aggregates;
using Marlin.Orders.Contracts.Commands;
using Marlin.Orders.Services;
using Wolverine.Marten;

namespace Marlin.Orders.Commands;

public static class UpdateBestBidAggregateHandler
{
    public static Events Handle(UpdateBestBid command, OrderBook state, ILogger logger)
    {
        try
        {
            logger.LogInformation("Command: {@Command}", command);
            var events = new Events();
            events += OrderBookService.Execute(command, state);
            return events;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error executing command: {nameof(UpdateBestBid)}");
            throw;
        }
    }
}