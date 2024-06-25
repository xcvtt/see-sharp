using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Exceptions;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres.Interfaces;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbConnection _dbConnection;
    private DbTransaction? _dbTransaction;
    
    public UnitOfWork(DbDataSource dbDataSource)
    {
        _dbConnection = dbDataSource.CreateConnection();
    }

    public async Task Begin(CancellationToken token)
    {
        await _dbConnection.OpenAsync(token);
        _dbTransaction = await _dbConnection.BeginTransactionAsync(token);
    }

    public async Task Commit(CancellationToken token)
    {
        if (_dbTransaction is null)
        {
            throw new InfrastructureException($"{nameof(_dbTransaction)} was null");
        }
        
        await _dbTransaction.CommitAsync(token);
    }

    public async Task Rollback(CancellationToken token)
    {
        if (_dbTransaction is null)
        {
            throw new InfrastructureException($"{nameof(_dbTransaction)} was null");
        }
        
        await _dbTransaction.RollbackAsync(token);
    }

    public DbConnection Connection
    {
        get
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                throw new InfrastructureException("Connection wasn't open");
            }
            
            return _dbConnection;
        }
    }

    public DbTransaction? Transaction
    {
        get
        {
            if (_dbConnection.State != ConnectionState.Open)
            {
                throw new InfrastructureException("Connection wasn't open");
            }

            return _dbTransaction;
        }
    }


    public async ValueTask DisposeAsync()
    {
        await _dbConnection.DisposeAsync();

        if (_dbTransaction is not null)
        {
            await _dbTransaction.DisposeAsync();
        }
    }
}