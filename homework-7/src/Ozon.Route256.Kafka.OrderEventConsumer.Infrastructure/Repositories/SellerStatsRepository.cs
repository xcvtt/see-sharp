using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public class SellerStatsRepository : ISellerStatsRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public SellerStatsRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<long[]> AddOrUpdate(SellerStatsEntityV1[] stats, CancellationToken token)
    {
        var sqlQuery = $"""
                        insert into seller_stats as ss (seller_id, item_id, currency, sales, total_due)
                        select seller_id, item_id, currency, sales, total_due
                        from UNNEST(@Stats)
                        on conflict (seller_id, item_id, currency)
                        do update
                        set sales = ss.sales + excluded.sales,
                            total_due = ss.total_due + excluded.total_due
                        returning item_id;
                        """;

        var cmd = new CommandDefinition(
            sqlQuery,
            new
            {
                Stats = stats,
            },
            cancellationToken: token,
            transaction: _unitOfWork.Transaction);

        var ids = await _unitOfWork.Connection.QueryAsync<long>(cmd);
        
        return ids.ToArray();
    }
}