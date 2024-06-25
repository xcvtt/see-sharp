using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services.Interfaces;

public interface IStatsService
{
    Task UpdateStats(IList<OrderEvent> orderEvents, CancellationToken token);
}