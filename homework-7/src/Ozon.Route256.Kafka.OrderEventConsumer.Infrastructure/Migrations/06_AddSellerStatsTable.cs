using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(6, "Add SellerStats table")]
public sealed class AddSellerStatsTable : SqlMigration {
    private const string TableName = "seller_stats";
    
    protected override string GetUpSql(IServiceProvider services)
    {
        return $"""
                create table if not exists {TableName} (
                      seller_id bigint not null
                    , item_id   bigint not null
                    , currency  text not null
                    , sales     bigint not null
                    , total_due bigint not null
                    , primary key(seller_id, item_id, currency)
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