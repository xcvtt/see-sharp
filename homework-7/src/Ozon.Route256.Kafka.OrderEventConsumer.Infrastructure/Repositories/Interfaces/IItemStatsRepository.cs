using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface IItemStatsRepository
{
    Task<long[]> AddOrUpdate(ItemStatsEntityV1[] stats, CancellationToken token);
}