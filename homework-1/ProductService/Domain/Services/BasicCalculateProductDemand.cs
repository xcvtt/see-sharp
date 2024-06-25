using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class BasicCalculateProductDemand : ICalculateProductDemand
{
    private readonly IProductDailyHistoryRepository _productDailyHistoryRepository;
    private readonly ICalculateSalesPrediction _calculateSalesPrediction;

    public BasicCalculateProductDemand(
        IProductDailyHistoryRepository productDailyHistoryRepository,
        ICalculateSalesPrediction calculateSalesPrediction)
    {
        ArgumentNullException.ThrowIfNull(productDailyHistoryRepository);
        ArgumentNullException.ThrowIfNull(calculateSalesPrediction);
        
        _productDailyHistoryRepository = productDailyHistoryRepository;
        _calculateSalesPrediction = calculateSalesPrediction;
    }
    
    public decimal CalculateProductDemand(long productId, int days)
    {
        var predictedSales = _calculateSalesPrediction.CalculateSalesPrediction(productId, days);

        var lastProduct = _productDailyHistoryRepository.GetProductDailyHistory(productId)
            .MaxBy(x => x.Day);

        if (lastProduct is null)
        {
            throw DomainException.ProductNotFound(productId);
        }

        var currentProductStock = lastProduct.Stock.Value;

        var predictedDemand = predictedSales - currentProductStock;

        return Math.Max(predictedDemand, 0M);
    }
}