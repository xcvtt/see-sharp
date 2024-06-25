using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<OrderEntityV1[]> AddReturn(OrderEntityV1[] orders, CancellationToken token);
}