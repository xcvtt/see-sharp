using Application.Services;
using DataAccess.Constants;
using DataAccess.Repository;
using Domain.Repository;
using Domain.Services;
using WebAPI.Options;

namespace WebAPI.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static void AddProductService(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var pathOptions = configuration.GetSection(RepositoryFilePathOptions.RepositoryFilePath)
            .Get<RepositoryFilePathOptions>();
        
        ArgumentNullException.ThrowIfNull(pathOptions);
        
        serviceCollection.AddSingleton<IProductDailyHistoryRepository, ProductDailyHistoryRepository>(x =>
            new ProductDailyHistoryRepository(pathOptions.ProductDailyHistoryFilePath));
        serviceCollection.AddSingleton<IProductSeasonalCoefficientRepository>(x =>
            new ProductSeasonalCoefficientRepository(pathOptions.ProductSeasonalCoeffsFilePath));

        serviceCollection.AddSingleton<ICalculateAverageDailySales, BasicCalculateAverageDailySales>();
        serviceCollection.AddSingleton<ICalculateSalesPrediction, BasicCalculateSalesPrediction>();
        serviceCollection.AddSingleton<ICalculateProductDemand, BasicCalculateProductDemand>();
        
        serviceCollection.AddSingleton<ISalesPredictionService, SalesPredictionService>();
    }
}