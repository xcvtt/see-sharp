using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ProductId
{
    public ProductId(long value)
    {
        if (value < 0)
        {
            throw new InvalidProductIdException(value);
        }
        
        Value = value;
    }
    
    public long Value { get; }
}