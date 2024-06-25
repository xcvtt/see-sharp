using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Handlers;

public class OrderEventHandler : IHandler<long, OrderEvent>
{
    private readonly ILogger<OrderEventHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private int _handledMessagesCounter;

    public OrderEventHandler(ILogger<OrderEventHandler> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Handle(IReadOnlyCollection<ConsumeResult<long, OrderEvent>> messages, CancellationToken token)
    {
        var orderEvents = messages.Select(x => x.Message.Value).ToList();
        
        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        var statsService = scope.ServiceProvider.GetRequiredService<IStatsService>();
        
        try
        {
            await statsService.UpdateStats(orderEvents, token);

            _handledMessagesCounter += messages.Count;
            _logger.LogInformation("Handled {Count} messages", _handledMessagesCounter);
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception thrown {Message}", ex.Message);
            throw;
        }
    }
}
