using System.Collections.Immutable;
using DataAccess.Parser;
using Domain.Entities;
using Domain.Repository;

namespace DataAccess.Repository;

public class ProductDailyHistoryRepository(string filePath) : IProductDailyHistoryRepository
{
    private readonly List<ProductDailyHistory> _productsDailyHistory = FileParser.ParseDailyHistory(filePath);

    public IReadOnlyCollection<ProductDailyHistory> GetProductDailyHistory(long productId)
    {
        return _productsDailyHistory.Where(x => x.ProductId.Value == productId).ToImmutableList();
    }
}