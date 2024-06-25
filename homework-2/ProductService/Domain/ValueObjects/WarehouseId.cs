using Domain.Exceptions;

namespace Domain.ValueObjects;

public record WarehouseId
{
    public WarehouseId(long value)
    {
        if (value < 0)
        {
            throw new InvalidWarehouseIdException(value);
        }
        
        Value = value;
    }

    public long Value { get; }
}