using Domain.Entities;

namespace Domain.Repository;

public interface IProductSeasonalCoefficientRepository
{
    IReadOnlyCollection<ProductSeasonalCoefficient> GetProductSeasonalCoefficients(long productId);
}