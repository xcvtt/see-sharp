using Domain.Entities;
using Domain.Repository;
using Domain.Services;
using Domain.ValueObjects;
using Moq;

namespace DomainUnitTests.Fixtures;

public sealed class SalesPredictionServiceFixture : IDisposable
{
    public ICalculateAverageDailySales CalculateAverageDailySales { get; }
    public ICalculateSalesPrediction CalculateSalesPrediction { get; }
    public ICalculateProductDemand CalculateProductDemand { get; }
    
    public SalesPredictionServiceFixture()
    {
        var productHistory = new Mock<IProductDailyHistoryRepository>();
        var productCoeffs = new Mock<IProductSeasonalCoefficientRepository>();
        
        PrepareProductHistory(productHistory);
        PrepareProductCoeffs(productCoeffs);

        CalculateAverageDailySales = new BasicCalculateAverageDailySales(productHistory.Object);
        CalculateSalesPrediction = 
            new BasicCalculateSalesPrediction(productHistory.Object, productCoeffs.Object, CalculateAverageDailySales);
        CalculateProductDemand = new BasicCalculateProductDemand(productHistory.Object, CalculateSalesPrediction);
    }

    private static void PrepareProductHistory(Mock<IProductDailyHistoryRepository> mock)
    {
        mock.DefaultValue = DefaultValue.Mock;
        
        mock.Setup(x => x.GetProductDailyHistory(1)).Returns(
            new List<ProductDailyHistory>
            {
                GetProductHistory(1, new DateOnly(2023, 1, 1), 10, 100),
                GetProductHistory(1, new DateOnly(2023, 1, 2), 30, 90),
            });
        
        mock.Setup(x => x.GetProductDailyHistory(2)).Returns(
            new List<ProductDailyHistory>
            {
                GetProductHistory(2, new DateOnly(2023, 1, 1), 30, 40),
                GetProductHistory(2, new DateOnly(2023, 1, 2), 0, 40),
                GetProductHistory(2, new DateOnly(2023, 1, 3), 0, 40),
            });
        
        mock.Setup(x => x.GetProductDailyHistory(3)).Returns(
            new List<ProductDailyHistory>
            {
                GetProductHistory(3, new DateOnly(2023, 1, 1), 0, 10),
                GetProductHistory(3, new DateOnly(2023, 1, 2), 0, 20),
            });
        
        mock.Setup(x => x.GetProductDailyHistory(4)).Returns(
            new List<ProductDailyHistory>
            {
                GetProductHistory(4, new DateOnly(2023, 1, 1), 50, 50),
                GetProductHistory(4, new DateOnly(2023, 1, 2), 0, 0),
            });
        
        mock.Setup(x => x.GetProductDailyHistory(5)).Returns(
            new List<ProductDailyHistory>
            {
                GetProductHistory(5, new DateOnly(2023, 1, 1), 0, 0),
                GetProductHistory(5, new DateOnly(2023, 1, 2), 0, 0),
            });
        
        
    }
    
    private static void PrepareProductCoeffs(Mock<IProductSeasonalCoefficientRepository> mock)
    {
        mock.DefaultValue = DefaultValue.Mock;

        mock.Setup(x => x.GetProductSeasonalCoefficients(1)).Returns(
            new List<ProductSeasonalCoefficient>
        {
            GetProductCoeff(1, 1, 0.5M),
            GetProductCoeff(1, 2, 1M),
            GetProductCoeff(1, 3, 2M),
        });
        
        mock.Setup(x => x.GetProductSeasonalCoefficients(2)).Returns(
            new List<ProductSeasonalCoefficient>
            {
                GetProductCoeff(2, 1, 3M),
                GetProductCoeff(2, 2, 0M),
            });
    }

    private static ProductSeasonalCoefficient GetProductCoeff(long productId, int month, decimal coeff)
    {
        return new ProductSeasonalCoefficient(new ProductId(productId), new Month(month), new SeasonalCoefficient(coeff));
    }

    private static ProductDailyHistory GetProductHistory(long productId, DateOnly date, int sales, int stock)
    {
        return new ProductDailyHistory(new ProductId(productId), date, new ProductSales(sales), new ProductStock(stock));
    }
    
    public void Dispose()
    {
    }
}