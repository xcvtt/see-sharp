using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class BasicCalculateSalesPrediction : ICalculateSalesPrediction
{
    private readonly IProductDailyHistoryRepository _productDailyHistoryRepository;
    private readonly IProductSeasonalCoefficientRepository _productSeasonalCoefficientRepository;
    private readonly ICalculateAverageDailySales _calculateAverageDailySales;

    public BasicCalculateSalesPrediction(
        IProductDailyHistoryRepository productDailyHistoryRepository,
        IProductSeasonalCoefficientRepository productSeasonalCoefficientRepository,
        ICalculateAverageDailySales calculateAverageDailySales)
    {
        ArgumentNullException.ThrowIfNull(productDailyHistoryRepository);
        ArgumentNullException.ThrowIfNull(productSeasonalCoefficientRepository);
        ArgumentNullException.ThrowIfNull(calculateAverageDailySales);
        
        _productDailyHistoryRepository = productDailyHistoryRepository;
        _productSeasonalCoefficientRepository = productSeasonalCoefficientRepository;
        _calculateAverageDailySales = calculateAverageDailySales;
    }
    
    public decimal CalculateSalesPrediction(long productId, int days)
    {
        var productAds = _calculateAverageDailySales.CalculateAverageDailySales(productId);

        if (productAds == 0M)
        {
            return 0M;
        }

        var productSeasonalCoefficients =
            _productSeasonalCoefficientRepository.GetProductSeasonalCoefficients(productId);

        var monthCoefficientDic = productSeasonalCoefficients
            .ToDictionary(k => k.Month.Value, v => v.Coefficient.Value);
        
        var lastProduct = _productDailyHistoryRepository.GetProductDailyHistory(productId)
            .MaxBy(x => x.Day);

        if (lastProduct is null)
        {
            throw DomainException.ProductNotFound(productId);
        }

        var lastDay = lastProduct.Day;

        var predictedSales = 0M;

        for (var i = 0; i < days; i++)
        {
            lastDay = lastDay.AddDays(1);
            
            var currentMonthCoefficient = 1M;   
            
            if (monthCoefficientDic.TryGetValue(lastDay.Month, out var value))
            {
                currentMonthCoefficient = value;
            }

            predictedSales += productAds * currentMonthCoefficient;
        }

        return predictedSales;
    }
}