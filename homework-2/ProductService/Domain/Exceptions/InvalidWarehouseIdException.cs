namespace Domain.Exceptions;

public class InvalidWarehouseIdException(long warehouseId) 
    : DomainException($"Invalid warehouse id: {warehouseId}");