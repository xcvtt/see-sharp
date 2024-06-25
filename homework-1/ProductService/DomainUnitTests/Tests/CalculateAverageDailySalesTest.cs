using Domain.Exceptions;
using DomainUnitTests.Fixtures;
using Xunit;

namespace DomainUnitTests.Tests;

public class CalculateAverageDailySalesTest : IClassFixture<SalesPredictionServiceFixture>
{
    private SalesPredictionServiceFixture _fixture;

    public CalculateAverageDailySalesTest(SalesPredictionServiceFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(777)]
    public void CalculateAverageDailySales_NoProductStock_ThrowsDomainException(long productId)
    {
        Assert.Throws<ProductNotFoundException>(() => _fixture.CalculateAverageDailySales.CalculateAverageDailySales(productId));
    }

    public static TheoryData<long, decimal> StockExistsData => new()
    {
        { 1, 20M },
        { 2, 10M },
        { 3, 0M },
        { 4, 50M },
    };
    
    [Theory]
    [MemberData(nameof(StockExistsData))]
    public void CalculateAverageDailySales_StockExists_CorrectAverage(long productId, decimal expectedAds)
    {
        var ads = _fixture.CalculateAverageDailySales.CalculateAverageDailySales(productId);

        Assert.Equal(expectedAds, ads);
    }
}