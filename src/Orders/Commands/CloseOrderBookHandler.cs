using Marlin.Orders.Aggregates;
using Marlin.Orders.Contracts.Commands;
using Marlin.Orders.Services;
using Wolverine.Marten;

namespace Marlin.Orders.Commands;

public static class CloseOrderBookAggregateHandler
{
    public static Events Handle(CloseOrderBook command, OrderBook state, ILogger logger)
    {
        try
        {
            var events = new Events();
            events += OrderBookService.Execute(command, state);
            return events;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error executing command: {nameof(CloseOrderBook)}");
            throw;
        }
    }
}