using Domain.Exceptions;

namespace Domain.ValueObjects;

public sealed record ProductId
{
    public long Value { get; }

    public ProductId(long productId)
    {
        if (productId < 0)
        {
            throw DomainException.InvalidProductId(productId);
        }

        Value = productId;
    }
}