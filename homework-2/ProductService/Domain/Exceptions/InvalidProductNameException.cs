namespace Domain.Exceptions;

public class InvalidProductNameException(string productName) 
    : DomainException($"Invalid product name: {productName}");