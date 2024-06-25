using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record SeasonalCoefficient
{
    private const decimal MinCoefficient = 0;
    private const decimal MaxCoefficient = 3;
    
    public decimal Value { get; }

    public SeasonalCoefficient(decimal value)
    {
        if (value is < MinCoefficient or > MaxCoefficient)
        {
            throw DomainException.InvalidSeasonalCoefficient(value);
        }

        Value = value;
    }
}