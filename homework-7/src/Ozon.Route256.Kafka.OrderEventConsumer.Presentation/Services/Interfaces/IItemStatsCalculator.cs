using System.Collections.Generic;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Models;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services.Interfaces;

public interface IItemStatsCalculator
{
    IList<ItemStats> CalculateItemStats(IList<OrderEvent> orderEvents);
}