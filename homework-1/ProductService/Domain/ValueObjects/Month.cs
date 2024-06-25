using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record Month
{
    private const int FirstMonth = 1;
    private const int LastMonth = 12;
    
    public int Value { get; }
    
    public Month(int value)
    {
        if (value is < FirstMonth or > LastMonth)
        {
            throw DomainException.InvalidMonth(value);
        }

        Value = value;
    }
}