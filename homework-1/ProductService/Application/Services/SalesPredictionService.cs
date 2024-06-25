using Application.Dtos;
using Domain.Exceptions;
using Domain.Services;

namespace Application.Services;

public class SalesPredictionService : ISalesPredictionService
{
    private readonly ICalculateAverageDailySales _calculateAverageDailySales;
    private readonly ICalculateSalesPrediction _calculateSalesPrediction;
    private readonly ICalculateProductDemand _calculateProductDemand;

    public SalesPredictionService(
        ICalculateAverageDailySales calculateAverageDailySales,
        ICalculateSalesPrediction calculateSalesPrediction,
        ICalculateProductDemand calculateProductDemand)
    {
        ArgumentNullException.ThrowIfNull(calculateAverageDailySales);
        ArgumentNullException.ThrowIfNull(calculateSalesPrediction);
        ArgumentNullException.ThrowIfNull(calculateProductDemand);
        
        _calculateAverageDailySales = calculateAverageDailySales;
        _calculateSalesPrediction = calculateSalesPrediction;
        _calculateProductDemand = calculateProductDemand;
    }

    public CalculateAds CalculateAverageDailySales(long productId)
    {
        var ads = _calculateAverageDailySales.CalculateAverageDailySales(productId);

        return new CalculateAds(productId, ads);
    }

    public CalculatePrediction CalculateSalesPrediction(long productId, int days)
    {
        var predictedSales = _calculateSalesPrediction.CalculateSalesPrediction(productId, days);

        return new CalculatePrediction(productId, days, predictedSales);
    }

    public CalculateDemand CalculateProductDemand(long productId, int days)
    {
        var demand = _calculateProductDemand.CalculateProductDemand(productId, days);

        return new CalculateDemand(productId, days, demand);
    }

    public Task<CalculateAds> CalculateAverageDailySalesAsync(long productId, CancellationToken token)
    {
        return Task.Run(() => CalculateAverageDailySales(productId), token);
    }

    public Task<CalculatePrediction> CalculateSalesPredictionAsync(long productId, int days, CancellationToken token)
    {
        return Task.Run(() => CalculateSalesPrediction(productId, days), token);
    }

    public Task<CalculateDemand> CalculateProductDemandAsync(long productId, int days, CancellationToken token)
    {
        return Task.Run(() => CalculateProductDemand(productId, days), token);
    }
}