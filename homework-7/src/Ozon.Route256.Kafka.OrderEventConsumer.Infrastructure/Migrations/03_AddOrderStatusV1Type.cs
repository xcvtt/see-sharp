using System;
using FluentMigrator;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

namespace Ozon.Route256.Postgres.Persistence.Migrations;

[Migration(3, "Add OrderStatusV1 type")]
public sealed class AddOrderStatusV1Type : SqlMigration {
    private const string TypeName = "order_status_v1";
    
    protected override string GetUpSql(IServiceProvider services)
    {
        return $"""
                DO $$
                    BEGIN
                        IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = '{TypeName}') THEN
                            CREATE TYPE {TypeName} as ENUM ('created', 'delivered', 'cancelled');
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