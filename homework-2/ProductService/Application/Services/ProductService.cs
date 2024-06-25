using System.Collections.Immutable;
using Application.Dtos;
using Application.Exceptions;
using Application.Mapping;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Domain.ValueObjects;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public ProductDto AddProduct(
        string name,
        decimal price,
        decimal weight,
        ProductType type,
        DateTime created,
        long warehouseId)
    {
        var id = Random.Shared.Next();
        
        var product = new Product(
            new ProductId(id),
            new ProductName(name),
            new ProductPrice(price),
            new ProductWeight(weight),
            type,
            created,
            new WarehouseId(warehouseId));

        var existingProduct = _productRepository.GetById(id);

        if (existingProduct is not null)
        {
            throw new ProductExistsException(id);
        }

        _productRepository.Add(product);

        return product.AsDto();
    }

    public ProductDto RemoveProduct(long productId)
    {
        var product = _productRepository.GetById(productId);

        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }

        _productRepository.Remove(product);

        return product.AsDto();
    }

    public ProductDto GetProduct(long productId)
    {
        var product = _productRepository.GetById(productId);

        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }

        return product.AsDto();
    }

    public ProductDto UpdateProductPrice(long productId, decimal newPrice)
    {
        var product = _productRepository.GetById(productId);

        if (product is null)
        {
            throw new ProductNotFoundException(productId);
        }

        var newProduct = new Product(
            product.ProductId,
            product.ProductName,
            new ProductPrice(newPrice),
            product.ProductWeight,
            product.ProductType,
            product.ProductCreatedDate,
            product.WarehouseId);

        _productRepository.Update(newProduct);

        return newProduct.AsDto();
    }

    public IReadOnlyCollection<ProductDto> GetFilteredProducts(
        int page,
        int pageSize,
        ProductType? type,
        DateTime? created,
        long? warehouseId)
    {
        var products = _productRepository.GetProducts().AsQueryable();

        if (type is not null)
        {
            products = products.Where(x => x.ProductType == type);
        }

        if (created is not null)
        {
            products = products.Where(x => x.ProductCreatedDate == created);
        }

        if (warehouseId is not null)
        {
            products = products.Where(x => x.WarehouseId.Value == warehouseId);
        }

        return products.Skip((page - 1) * pageSize).Take(pageSize).Select(x => x.AsDto()).ToImmutableList();
    }
}