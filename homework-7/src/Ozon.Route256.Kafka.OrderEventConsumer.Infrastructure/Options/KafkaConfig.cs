namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Options;

public record KafkaConfig
{
    public required string Servers { get; init; }
    public required string GroupId { get; init; }
    public required string Topic { get; init; }
    public required int ChannelCapacity { get; init; }
    public required int BufferDelaySeconds { get; init; }
    public required int MaxRetryAttempts { get; init; }
    public required int RetryTimeoutSeconds { get; init; }
    public required int RetryDelaySeconds { get; init; }
}