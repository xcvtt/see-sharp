using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record ProductStock
{
    public int Value { get; }

    public ProductStock(int productSales)
    {
        if (productSales < 0)
        {
            throw DomainException.InvalidProductStock(productSales);
        }

        Value = productSales;
    }
}