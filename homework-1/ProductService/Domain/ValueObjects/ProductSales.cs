using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record ProductSales
{
    public int Value { get; }

    public ProductSales(int productSales)
    {
        if (productSales < 0)
        {
            throw DomainException.InvalidProductSales(productSales);
        }

        Value = productSales;
    }
}