using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class BasicCalculateAverageDailySales : ICalculateAverageDailySales
{
    private readonly IProductDailyHistoryRepository _productDailyHistoryRepository;

    public BasicCalculateAverageDailySales(IProductDailyHistoryRepository productDailyHistoryRepository)
    {
        ArgumentNullException.ThrowIfNull(productDailyHistoryRepository);

        _productDailyHistoryRepository = productDailyHistoryRepository;
    }
    
    public decimal CalculateAverageDailySales(long productId)
    {
        var productsInStock = _productDailyHistoryRepository.GetProductDailyHistory(productId)
            .Where(x => x.Stock.Value > 0)
            .ToList();

        if (!productsInStock.Any())
        {
            throw DomainException.ProductNotFound(productId);
        }

        decimal totalSales = productsInStock.Sum(x => x.Sales.Value);

        return totalSales / productsInStock.Count;
    }
}