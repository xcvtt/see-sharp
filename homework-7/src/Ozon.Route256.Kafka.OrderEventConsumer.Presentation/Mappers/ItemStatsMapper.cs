using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Mappers;

public static class ItemStatsMapper
{
    public static ItemStatsEntityV1 AsDbEntity(this ItemStats itemStats)
    {
        return new ItemStatsEntityV1
        {
            ItemId = itemStats.ItemId,
            Reserved = itemStats.Reserved,
            Sold = itemStats.Sold,
            Cancelled = itemStats.Cancelled,
            At = itemStats.At,
        };
    }
}