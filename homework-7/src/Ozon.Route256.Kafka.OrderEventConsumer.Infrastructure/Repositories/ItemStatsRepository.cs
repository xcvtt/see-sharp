using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public class ItemStatsRepository : IItemStatsRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public ItemStatsRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<long[]> AddOrUpdate(ItemStatsEntityV1[] stats, CancellationToken token)
    {
        var sqlQuery = $"""
                        insert into item_stats as iss (item_id, reserved, sold, cancelled, at)
                        select item_id, reserved, sold, cancelled, at
                        from UNNEST(@Stats)
                        on conflict (item_id) 
                        do update
                        set reserved = iss.reserved + excluded.reserved,
                            sold = iss.sold + excluded.sold,
                            cancelled = iss.cancelled + excluded.cancelled,
                            at = excluded.at
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