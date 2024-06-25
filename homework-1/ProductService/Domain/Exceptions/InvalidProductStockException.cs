namespace Domain.Exceptions;

public class InvalidProductStockException(string message) : DomainException(message);