using System.Net;
using HomeworkApp.Bll.Services;
using HomeworkApp.Bll.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace HomeworkApp.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(
        this IServiceCollection services,
        List<RedLockEndPoint> redLockEndPoints)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddSingleton<IDistributedLockFactory, RedLockFactory>(x => 
            RedLockFactory.Create(redLockEndPoints));
        services.AddScoped<IRateLimiterService, RateLimiterService>();
        
        return services;
    }
}