namespace Domain.Exceptions;

public class InvalidProductIdException(string message) : DomainException(message);