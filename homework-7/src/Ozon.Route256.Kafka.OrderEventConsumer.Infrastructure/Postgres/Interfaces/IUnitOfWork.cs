using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    Task Begin(CancellationToken token);
    Task Commit(CancellationToken token);
    Task Rollback(CancellationToken token);
    DbConnection Connection { get; }
    DbTransaction? Transaction { get; }
}