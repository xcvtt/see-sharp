using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(1, "Add ItemStatsV1 type")]
public sealed class AddItemStatsV1Type : SqlMigration
{
    private const string TypeName = "item_stats_v1";
    
    protected override string GetUpSql(IServiceProvider services)
    {
        return $"""
               DO $$
                   BEGIN
                       IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = '{TypeName}') THEN
                           CREATE TYPE {TypeName} as
                           (
                                 item_id    bigint
                               , reserved   bigint
                               , sold       bigint
                               , cancelled  bigint
                               , at         timestamp with time zone
                           );
                       END IF;
                   END
               $$;
               """;
    }

    protected override string GetDownSql(IServiceProvider services)
    {
        return $"""
               DO $$
                   BEGIN
                       DROP TYPE IF EXISTS {TypeName};
                   END
               $$;
               """;
    }
}