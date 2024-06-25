using Domain.ValueObjects;

namespace Application.Dtos;

public record ProductDto(
    long Id,
    string Name,
    decimal Price,
    ProductType Type,
    decimal Weight,
    DateTime Created,
    long WarehouseId);
    