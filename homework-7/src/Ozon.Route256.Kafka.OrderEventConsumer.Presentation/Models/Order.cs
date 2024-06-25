namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;

public record Order
{
    public required long OrderId { get; init; }
    public required OrderStatus Status { get; init; }
}