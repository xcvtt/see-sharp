using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ProductPrice
{
    public ProductPrice(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidProductPriceException(value);
        }
        
        Value = value;
    }

    public decimal Value { get; }
}