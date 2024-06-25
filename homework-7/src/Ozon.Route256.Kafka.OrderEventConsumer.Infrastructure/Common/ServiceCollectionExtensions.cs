using System;
using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql.NameTranslation;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Postgres.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories.Interfaces;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFluentMigrator(
        this IServiceCollection services,
        Assembly assembly)
    {
        var pgConfig = services.BuildServiceProvider().GetRequiredService<IOptions<PostgresConfig>>();
        
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(
                builder => builder
                    .AddPostgres()
                    .ScanIn(assembly).For.Migrations())
            .AddOptions<ProcessorOptions>()
            .Configure(
                options =>
                {
                    options.ProviderSwitches = "Force Quote=false";
                    options.Timeout = TimeSpan.FromMinutes(10);
                    options.ConnectionString = pgConfig.Value.ConnectionString;
                });

        return services;
    }
    
    public static IServiceCollection MapCompositeTypes(
        this IServiceCollection services)
    {
        var pgConfig = services.BuildServiceProvider().GetRequiredService<IOptions<PostgresConfig>>();
        var translator = new NpgsqlSnakeCaseNameTranslator();
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.AddNpgsqlDataSource(
            pgConfig.Value.ConnectionString,
            x =>
            {
                x.MapComposite<ItemStatsEntityV1>("item_stats_v1", translator);
                x.MapComposite<SellerStatsEntityV1>("seller_stats_v1", translator);
                x.MapComposite<OrderEntityV1>("order_v1", translator);
                x.MapEnum<OrderStatusV1>("order_status_v1", translator);
                x.UseLoggerFactory(new NullLoggerFactory());
            });
        
        return services;
    }

    public static IServiceCollection AddDataAccess(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IItemStatsRepository, ItemStatsRepository>();
        services.AddScoped<ISellerStatsRepository, SellerStatsRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}
