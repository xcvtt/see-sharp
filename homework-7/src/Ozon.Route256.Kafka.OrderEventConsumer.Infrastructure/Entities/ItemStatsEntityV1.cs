using System;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;

public record ItemStatsEntityV1
{
    public required long ItemId { get; init; }
    public required long Reserved { get; init; }
    public required long Sold { get; init; }
    public required long Cancelled { get; init; }
    public required DateTimeOffset At { get; init; }
}