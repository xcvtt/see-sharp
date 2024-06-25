using Application.Services.Interfaces;
using Grpc.Core;
using WebAPI.Grpc;
using WebAPI.Mapping;

namespace WebAPI.Services;

public class ProductGrpcService : ProductGrpc.ProductGrpcBase
{
    private readonly IProductService _productService;

    public ProductGrpcService(IProductService productService)
    {
        _productService = productService;
    }

    public override Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var productDto = _productService.GetProduct(request.ProductId);

        return Task.FromResult(new GetProductResponse{ Product = productDto.AsGrpcProduct() });
    }

    public override Task<AddProductResponse> AddProduct(AddProductRequest request, ServerCallContext context)
    {
        var productDto = _productService.AddProduct(
            request.ProductName,
            request.ProductPrice,
            request.ProductWeight,
            (Domain.ValueObjects.ProductType)request.ProductType,
            request.CreatedDate.ToDateTime(),
            request.WarehouseId);
        
        return Task.FromResult(new AddProductResponse{ Product = productDto.AsGrpcProduct() });
    }

    public override Task<RemoveProductResponse> RemoveProduct(RemoveProductRequest request, ServerCallContext context)
    {
        var productDto = _productService.RemoveProduct(request.ProductId);

        return Task.FromResult(new RemoveProductResponse{ Product = productDto.AsGrpcProduct() });
    }

    public override Task<UpdateProductResponse> UpdateProductPrice(UpdateProductPriceRequest request, ServerCallContext context)
    {
        var productDto = _productService.UpdateProductPrice(request.ProductId, request.ProductPrice);

        return Task.FromResult(new UpdateProductResponse{ Product = productDto.AsGrpcProduct() });
    }

    public override Task<GetFilteredProductsResponse> GetFilteredProducts(GetFilteredProductsRequest request, ServerCallContext context)
    {
        var products = _productService.GetFilteredProducts(
            request.Page,
            request.PageSize,
            request.ProductType != ProductType.Unspecified ? (Domain.ValueObjects.ProductType)request.ProductType : null,
            request.CreatedDate?.ToDateTime(),
            request.WarehouseId)
            .Select(x => x.AsGrpcProduct())
            .ToList();
        
        return Task.FromResult(new GetFilteredProductsResponse{ Products = { products }});
    }
}