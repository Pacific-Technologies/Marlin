using Marlin.Orders.Aggregates;
using Marlin.Orders.Contracts.Commands;
using Marlin.Orders.Services;
using Wolverine.Marten;

namespace Marlin.Orders.Commands;

public static class OpenOrderBookHandler
{
    public static StartStream<OrderBook> Handle(OpenOrderBook command, ILogger logger)
    {
        try
        {
            logger.LogInformation(
                "{Symbol}: Opening order book at {UpdateId}",
                command.Symbol,
                command.UpdateId);
            return new StartStream<OrderBook>(
                command.OrderBookId,
                OrderBookService.Create(command));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error executing command: {nameof(OpenOrderBook)}");
            throw;
        }
    }
}