using Application.Services;
using Application.Services.Interfaces;
using DataAccess.Repositories;
using Domain.Repositories;

namespace WebAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IProductRepository, ProductRepository>();
        
        return serviceCollection;
    }

    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IProductService, ProductService>();
        
        return serviceCollection;
    }
}