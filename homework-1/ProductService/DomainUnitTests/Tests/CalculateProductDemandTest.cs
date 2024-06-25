using Domain.Exceptions;
using DomainUnitTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace DomainUnitTests.Tests;

public class CalculateProductDemandTest : IClassFixture<SalesPredictionServiceFixture>
{
    private SalesPredictionServiceFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public CalculateProductDemandTest(SalesPredictionServiceFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(5, 50)]
    [InlineData(6, 500)]
    public void CalculateProductDemand_NoStock_ThrowsDomainException(long productId, int days)
    {
        Assert.Throws<ProductNotFoundException>(() => _fixture.CalculateProductDemand.CalculateProductDemand(productId, days));
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 1)]
    public void CalculateProductDemand_PredictedLessThanStock_ReturnsZero(long productId, int days)
    {
        var demand = _fixture.CalculateProductDemand.CalculateProductDemand(productId, days);
        
        Assert.Equal(0M, demand);
    }
    
    public static TheoryData<long, int, decimal> PredictedMoreThanStock => new()
    {
        { 1, 50, 620M }, //29 * 20 * 0.5 + 21 * 20 * 1 = 710 (stock 90)
        { 1, 80, 1680M }, //29 * 20 * 0.5 + 28 * 20 * 1 + 23 * 20 * 2 = 1770 (stock 90)
        { 2, 50, 800M }, //28 * 10 * 3 + 21 * 10 * 0 = 840 (stock 40)
    };
    
    [Theory]
    [MemberData(nameof(PredictedMoreThanStock))]
    public void CalculateProductDemand_PredictedMoreThanStock_CorrectDemand(long productId, int days, decimal expectedDemand)
    {
        var demand = _fixture.CalculateProductDemand.CalculateProductDemand(productId, days);

        Assert.Equal(expectedDemand, demand);
    }
}