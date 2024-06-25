namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Options;

public record PostgresConfig
{
    public required string ConnectionString { get; set; }
}