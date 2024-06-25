namespace Domain.Exceptions;

public class InvalidProductIdException(long productId) 
    : DomainException($"Invalid product id: {productId}");