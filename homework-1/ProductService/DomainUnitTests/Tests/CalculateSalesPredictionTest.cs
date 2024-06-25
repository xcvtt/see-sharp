using Domain.Exceptions;
using DomainUnitTests.Fixtures;
using Xunit;

namespace DomainUnitTests.Tests;

public class CalculateSalesPredictionTest : IClassFixture<SalesPredictionServiceFixture>
{
    private SalesPredictionServiceFixture _fixture;

    public CalculateSalesPredictionTest(SalesPredictionServiceFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Theory]
    [InlineData(5, 1)]
    [InlineData(6, 5)]
    [InlineData(77, 10)]
    [InlineData(7777, 20)]
    public void CalculateSalesPrediction_NoStock_ThrowsDomainException(long productId, int days)
    {
        Assert.Throws<ProductNotFoundException>(() => _fixture.CalculateSalesPrediction.CalculateSalesPrediction(productId, days));
    }

    [Theory]
    [InlineData(3, 1)]
    [InlineData(3, 55)]
    public void CalculateSalesPrediction_ZeroAds_ReturnsZero(long productId, int days)
    {
        var pred = _fixture.CalculateSalesPrediction.CalculateSalesPrediction(productId, days);

        Assert.Equal(0M, pred);
    }
    
    public static TheoryData<long, int, decimal> SameMonthCoeffs => new()
    {
        { 1, 5, 50M },
        { 2, 10, 300M },
        { 3, 10, 0M },
    };
    
    [Theory]
    [MemberData(nameof(SameMonthCoeffs))]
    public void CalculateSalesPrediction_SameMonthCoeffs_CorrectPrediction(long productId, int days, decimal expectedPrediction)
    {
        var pred = _fixture.CalculateSalesPrediction.CalculateSalesPrediction(productId, days);

        Assert.Equal(expectedPrediction, pred);
    }
    
    public static TheoryData<long, int, decimal> SeveralMonthCoeffs => new()
    {
        { 1, 50, 710M }, //29 * 20 * 0.5 + 21 * 20 * 1 = 710
        { 1, 80, 1770M }, //29 * 20 * 0.5 + 28 * 20 * 1 + 23 * 20 * 2 = 1770
        { 2, 50, 840M }, //28 * 10 * 3 + 21 * 10 * 0 = 840
    };
    
    [Theory]
    [MemberData(nameof(SeveralMonthCoeffs))]
    public void CalculateSalesPrediction_SeveralMonthCoeffs_CorrectPrediction(long productId, int days, decimal expectedPrediction)
    {
        var pred = _fixture.CalculateSalesPrediction.CalculateSalesPrediction(productId, days);

        Assert.Equal(expectedPrediction, pred);
    }
}