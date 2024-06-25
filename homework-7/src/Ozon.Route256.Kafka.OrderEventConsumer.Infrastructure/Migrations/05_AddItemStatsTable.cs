using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(5, "Add ItemStats table")]
public sealed class AddItemStatsTable : SqlMigration
{
    private const string TableName = "item_stats";
    
    protected override string GetUpSql(IServiceProvider services)
    {
        return $"""
                create table if not exists {TableName} (
                      item_id    bigint primary key not null
                    , reserved   bigint not null
                    , sold       bigint not null
                    , cancelled  bigint not null
                    , at         timestamp with time zone not null
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