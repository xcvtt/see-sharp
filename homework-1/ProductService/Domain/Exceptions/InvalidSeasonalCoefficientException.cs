namespace Domain.Exceptions;

public class InvalidSeasonalCoefficientException(string message) : DomainException(message);