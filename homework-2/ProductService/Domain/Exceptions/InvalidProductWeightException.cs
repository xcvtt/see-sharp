namespace Domain.Exceptions;

public class InvalidProductWeightException(decimal productWeight) 
    : DomainException($"Invalid product weight: {productWeight}");