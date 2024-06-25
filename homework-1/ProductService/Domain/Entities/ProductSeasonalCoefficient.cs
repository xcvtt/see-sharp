using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class ProductSeasonalCoefficient
{
    public ProductId ProductId { get; }
    public Month Month { get; }
    public SeasonalCoefficient Coefficient { get; }

    public ProductSeasonalCoefficient(ProductId productId, Month month, SeasonalCoefficient coefficient)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ArgumentNullException.ThrowIfNull(month);
        ArgumentNullException.ThrowIfNull(coefficient);

        ProductId = productId;
        Month = month;
        Coefficient = coefficient;
    }
}