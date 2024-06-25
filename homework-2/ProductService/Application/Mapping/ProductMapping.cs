using Application.Dtos;
using Domain.Entities;

namespace Application.Mapping;

public static class ProductMapping
{
    public static ProductDto AsDto(this Product product)
        => new ProductDto(
            product.ProductId.Value,
            product.ProductName.Value,
            product.ProductPrice.Value,
            product.ProductType,
            product.ProductWeight.Value,
            product.ProductCreatedDate,
            product.WarehouseId.Value);
}