using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Handlers;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Kafka;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IItemStatsCalculator, ItemStatsCalculator>();
        services.AddSingleton<ISellerStatsCalculator, SellerStatsCalculator>();
        services.AddScoped<IStatsService, StatsService>();
        
        return services;
    }

    public static IServiceCollection AddKafkaConsumer<TKey, TValue>(this IServiceCollection services)
    {
        services.AddSingleton<KafkaAsyncConsumer<TKey, TValue>>();
        services.AddHostedService<KafkaBackgroundConsumer<TKey, TValue>>();

        return services;
    }

    public static IServiceCollection AddOrderEventConsumer(this IServiceCollection services)
    {
        services.AddSingleton<IHandler<long, OrderEvent>, OrderEventHandler>();
        services.AddSingleton<IDeserializer<OrderEvent>, SystemTextJsonDeserializer<OrderEvent>>(_ =>
            new SystemTextJsonDeserializer<OrderEvent>(new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            }));
        
        return services;
    }
}