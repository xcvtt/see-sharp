using Domain.Exceptions;

namespace Domain.ValueObjects;

public record ProductName
{
    private const int MaxLength = 50;
    
    public ProductName(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > MaxLength)
        {
            throw new InvalidProductNameException(value);
        }
        
        Value = value;
    }

    public string Value { get; }
}