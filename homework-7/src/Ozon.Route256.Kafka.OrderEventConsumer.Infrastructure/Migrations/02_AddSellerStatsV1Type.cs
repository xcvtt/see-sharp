using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(2, "Add SellerStatsV1 type")]
public sealed class AddSellerStatsV1Type : SqlMigration
{
    private const string TypeName = "seller_stats_v1";
    
    protected override string GetUpSql(IServiceProvider services)
    {
        return $"""
                DO $$
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = '{TypeName}') THEN
                            CREATE TYPE {TypeName} as
                            (
                                  seller_id bigint
                                , item_id   bigint
                                , currency  text
                                , sales     bigint
                                , total_due bigint
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