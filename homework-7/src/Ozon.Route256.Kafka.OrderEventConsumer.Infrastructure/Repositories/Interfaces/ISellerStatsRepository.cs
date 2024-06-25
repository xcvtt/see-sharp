using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface ISellerStatsRepository
{
    Task<long[]> AddOrUpdate(SellerStatsEntityV1[] stats, CancellationToken token);
}