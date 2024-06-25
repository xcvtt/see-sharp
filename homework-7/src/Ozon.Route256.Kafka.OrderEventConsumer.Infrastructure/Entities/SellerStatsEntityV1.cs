namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;

public record SellerStatsEntityV1
{
    public required long SellerId { get; init; }
    public required long ItemId { get; init; }
    public required string Currency { get; init; }
    public required long Sales { get; init; }
    public required long TotalDue { get; init; }
}