namespace Domain.Exceptions;

public class ProductNotFoundException(string message) : DomainException(message);