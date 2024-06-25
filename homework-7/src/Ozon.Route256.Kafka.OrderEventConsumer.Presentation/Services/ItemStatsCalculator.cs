using System;
using System.Collections.Generic;
using System.Linq;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services;

public class ItemStatsCalculator : IItemStatsCalculator
{
    public IList<ItemStats> CalculateItemStats(IList<OrderEvent> orderEvents)
    {
        var itemMap = new Dictionary<long, (long created, long delivered, long cancelled, DateTimeOffset lastAt)>();

        foreach (var orderEvent in orderEvents)
        {
            var orderStatus = orderEvent.Status;

            foreach (var item in orderEvent.Positions)
            {
                itemMap.TryGetValue(item.ItemId, out var stats);

                switch (orderStatus)
                {
                    case OrderEvent.OrderStatus.Created:
                        stats.created += item.Quantity;
                        break;
                    case OrderEvent.OrderStatus.Delivered:
                        stats.created -= item.Quantity;
                        stats.delivered += item.Quantity;
                        break;
                    case OrderEvent.OrderStatus.Canceled:
                        stats.created -= item.Quantity;
                        stats.cancelled += item.Quantity;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                stats.lastAt = orderEvent.Moment;

                itemMap[item.ItemId] = stats;
            }
        }

        return itemMap
            .Select(x => new ItemStats
            {
                ItemId = x.Key,
                Reserved = x.Value.created,
                Sold = x.Value.delivered,
                Cancelled = x.Value.cancelled,
                At = x.Value.lastAt
            })
            .OrderBy(x => x.ItemId)
            .ToList();
    }
}