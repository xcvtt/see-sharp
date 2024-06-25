using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    

    public async Task<OrderEntityV1[]> AddReturn(OrderEntityV1[] orders, CancellationToken token)
    {
        var sqlQuery = $"""
                        insert into orders (order_id, status)
                        select order_id, status
                        from UNNEST(@Orders)
                        on conflict do nothing 
                        returning order_id, status;
                        """;
        
        var cmd = new CommandDefinition(
            sqlQuery,
            new
            {
                Orders = orders,
            },
            cancellationToken: token,
            transaction: _unitOfWork.Transaction);

        var ids = await _unitOfWork.Connection.QueryAsync<OrderEntityV1>(cmd);
        
        return ids.ToArray();
    }
}