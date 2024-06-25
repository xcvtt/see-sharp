using Application.Dtos;
using Google.Protobuf.WellKnownTypes;
using WebAPI.Grpc;

namespace WebAPI.Mapping;

public static class ProductDtoMapping
{
    public static Product AsGrpcProduct(this ProductDto productDto)
    {
        return new Product
        {
            ProductId = productDto.Id,
            ProductName = productDto.Name,
            ProductPrice = productDto.Price,
            ProductWeight = productDto.Weight,
            CreatedDate = Timestamp.FromDateTime(productDto.Created.ToUniversalTime()),
            ProductType = (ProductType)productDto.Type,
            WarehouseId = productDto.WarehouseId,
        };
    }
}