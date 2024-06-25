namespace Domain.Exceptions;

public class InvalidProductSalesException(string message) : DomainException(message);