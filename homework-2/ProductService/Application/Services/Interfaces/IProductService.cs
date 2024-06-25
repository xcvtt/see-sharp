using Application.Dtos;
using Domain.ValueObjects;

namespace Application.Services.Interfaces;

public interface IProductService
{
    ProductDto AddProduct(
        string name,
        decimal price,
        decimal weight,
        ProductType type,
        DateTime created,
        long warehouseId);
    ProductDto RemoveProduct(long productId);
    ProductDto GetProduct(long productId);
    ProductDto UpdateProductPrice(long productId, decimal newPrice);
    IReadOnlyCollection<ProductDto> GetFilteredProducts(
        int page,
        int pageSize,
        ProductType? type,
        DateTime? created,
        long? warehouseId);
}