using Application.Dtos;

namespace Application.Services;

public interface ISalesPredictionService
{
    CalculateAds CalculateAverageDailySales(long productId);
    CalculatePrediction CalculateSalesPrediction(long productId, int days);
    CalculateDemand CalculateProductDemand(long productId, int days);
    
    Task<CalculateAds> CalculateAverageDailySalesAsync(long productId, CancellationToken token);
    Task<CalculatePrediction> CalculateSalesPredictionAsync(long productId, int days, CancellationToken token);
    Task<CalculateDemand> CalculateProductDemandAsync(long productId, int days, CancellationToken token);
}