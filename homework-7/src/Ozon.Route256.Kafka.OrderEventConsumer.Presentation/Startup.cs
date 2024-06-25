using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Extensions;
using Ozon.Route256.Kafka.OrderEventGenerator.Contracts;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging();

        services
            .Configure<PostgresConfig>(_configuration.GetSection(nameof(PostgresConfig)))
            .Configure<KafkaConfig>(_configuration.GetSection(nameof(KafkaConfig)));

        services
            .AddFluentMigrator(typeof(SqlMigration).Assembly)
            .MapCompositeTypes()
            .AddDataAccess()
            .AddAppServices();

        services
            .AddOrderEventConsumer()
            .AddKafkaConsumer<long, OrderEvent>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}
