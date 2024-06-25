using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ProductWeight
{
    public ProductWeight(decimal value)
    {
        if (value < 0)
        {
            throw new InvalidProductWeightException(value);
        }
        
        Value = value;
    }

    public decimal Value { get; }
}