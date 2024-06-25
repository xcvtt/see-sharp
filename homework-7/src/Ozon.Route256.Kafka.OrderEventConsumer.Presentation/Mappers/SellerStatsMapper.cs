using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Mappers;

public static class SellerStatsMapper
{
    public static SellerStatsEntityV1 AsDbEntity(this SellerStats sellerStats)
    {
        return new SellerStatsEntityV1
        {
            SellerId = sellerStats.SellerId,
            ItemId = sellerStats.ItemId,
            Currency = sellerStats.Currency,
            Sales = sellerStats.Sales,
            TotalDue = sellerStats.TotalDue,
        };
    }
}