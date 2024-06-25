using System.Collections.Immutable;
using DataAccess.Parser;
using Domain.Entities;
using Domain.Repository;

namespace DataAccess.Repository;

public class ProductSeasonalCoefficientRepository(string filePath) : IProductSeasonalCoefficientRepository
{
    private readonly List<ProductSeasonalCoefficient> _productsSeasonalCoefficients = FileParser.ParseSeasonalCoefficients(filePath);

    public IReadOnlyCollection<ProductSeasonalCoefficient> GetProductSeasonalCoefficients(long productId)
    {
        return _productsSeasonalCoefficients.Where(x => x.ProductId.Value == productId).ToImmutableList();
    }
}