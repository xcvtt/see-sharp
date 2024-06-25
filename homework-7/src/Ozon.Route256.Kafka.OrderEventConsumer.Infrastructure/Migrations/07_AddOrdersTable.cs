using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(7, "Add Orders table")]
public sealed class AddOrdersTable : SqlMigration {
    private const string TableName = "orders";
    
    protected override string GetUpSql(IServiceProvider services)
    {
        return $"""
                create table if not exists {TableName} (
                      order_id bigint not null
                    , status   order_status_v1 not null
                    , primary key(order_id, status)
                );
                """;
    }

    protected override string GetDownSql(IServiceProvider services)
    {
        return $"""
                drop table if exists {TableName}
                """;
    }
}