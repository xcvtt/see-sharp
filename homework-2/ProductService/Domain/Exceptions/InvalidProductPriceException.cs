namespace Domain.Exceptions;

public class InvalidProductPriceException(decimal productPrice) 
    : DomainException($"Invalid product price: {productPrice}");