using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Kafka;

public class KafkaBackgroundConsumer<TKey, TValue> : BackgroundService
{
    private readonly KafkaAsyncConsumer<TKey, TValue> _consumer;
    private readonly ILogger<KafkaBackgroundConsumer<TKey, TValue>> _logger;

    public KafkaBackgroundConsumer(
        KafkaAsyncConsumer<TKey, TValue> consumer,
        ILogger<KafkaBackgroundConsumer<TKey, TValue>> logger)
    {
        _consumer = consumer;
        _logger = logger;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occured");
        }
    }
}