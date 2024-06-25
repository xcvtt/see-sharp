namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;

public record OrderEntityV1
{
    public required long OrderId { get; init; }
    public required OrderStatusV1 Status { get; init; }
}