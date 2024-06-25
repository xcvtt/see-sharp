using System.Collections.Generic;
using System.Linq;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Mappers;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services;

public class SellerStatsCalculator : ISellerStatsCalculator
{
    public IList<SellerStats> CalculateSellerStats(IList<OrderEvent> orderEvents)
    {
        var sellerMap = new Dictionary<(long itemId, string currency), (long sales, long totalDue)>();

        foreach (var orderEvent in orderEvents)
        {
            if (orderEvent.Status != OrderEvent.OrderStatus.Delivered) continue;

            foreach (var item in orderEvent.Positions)
            {
                var key = (item.ItemId, item.Price.Currency);
                
                sellerMap.TryGetValue(key, out var stats);

                stats.sales += item.Quantity;
                stats.totalDue += item.Price.AsLongCents() * item.Quantity;

                sellerMap[key] = stats;
            }
        }

        return sellerMap
            .Select(x =>
            {
                var ids = ExtractSellerItemIds(x.Key.itemId);
                
                return new SellerStats
                {
                    SellerId = ids.sellerId,
                    ItemId = ids.itemId,
                    Currency = x.Key.currency,
                    Sales = x.Value.sales,
                    TotalDue = x.Value.totalDue,
                };
            })
            .OrderBy(x => x.SellerId)
            .ThenBy(x => x.ItemId)
            .ThenBy(x => x.Currency)
            .ToList();
    }

    private (long sellerId, long itemId) ExtractSellerItemIds(long itemIdFull)
    {
        const int sellerIdLength = 6;
        
        var itemIdString = itemIdFull.ToString();

        var sellerId = itemIdString[..sellerIdLength];
        var itemId = itemIdString[sellerIdLength..];

        return (int.Parse(sellerId), int.Parse(itemId));
    }
}