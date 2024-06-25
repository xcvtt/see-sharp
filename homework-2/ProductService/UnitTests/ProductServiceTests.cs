using Application.Exceptions;
using Application.Mapping;
using Application.Services;
using Application.Services.Interfaces;
using AutoFixture;
using AutoFixture.Xunit2;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit.Abstractions;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly IProductService _productService;
    private readonly ITestOutputHelper _outputHelper;

    public ProductServiceTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _productService = new ProductService(_productRepositoryMock.Object);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(666)]
    public void Get_ProductDoesntExist_ThrowsException(long productId)
    {
        // Act
        var act = () => _productService.GetProduct(productId);

        // Assert
        act.Should().Throw<ProductNotFoundException>();
    }

    [Theory, AutoData]
    public void Get_ProductExists_ReturnsValidProduct(Product product)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetById(product.ProductId.Value))
            .Returns(product);
        
        // Act
        var productDto = _productService.GetProduct(product.ProductId.Value);

        // Assert
        productDto.Should().NotBeNull();
        product.AsDto().Should().Be(productDto);
        _productRepositoryMock.Verify(x => x.GetById(product.ProductId.Value), Times.Once);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(666)]
    public void Remove_ProductDoesntExist_ThrowsException(long productId)
    {
        // Act
        var act = () => _productService.RemoveProduct(productId);

        // Assert
        act.Should().Throw<ProductNotFoundException>();
    }
    
    [Theory, AutoData]
    public void Remove_ProductExists_RemovesProductsReturnsValidDto(Product product)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetById(product.ProductId.Value))
            .Returns(product);
        
        // Act
        var productDto = _productService.RemoveProduct(product.ProductId.Value);

        // Assert
        productDto.Should().NotBeNull();
        product.AsDto().Should().Be(productDto);
        _productRepositoryMock.Verify(x => x.GetById(product.ProductId.Value), Times.Once);
        _productRepositoryMock.Verify(x => x.Remove(product), Times.Once);
    }

    [Theory, AutoData]
    public void AddProduct_ProductIdNotEqual_AddsProduct(Product product)
    {
        // Arrange
        var productDto = product.AsDto();
        
        // Act
        var addedProductDto = _productService.AddProduct(
            productDto.Name,
            productDto.Price,
            productDto.Weight,
            productDto.Type,
            productDto.Created,
            productDto.WarehouseId);

        // Assert
        _productRepositoryMock.Verify(x => x.GetById(addedProductDto.Id), Times.Once);
        _productRepositoryMock.Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
    }

    [Theory, AutoData]
    public void UpdateProductPrice_ProductDoesntExists_ThrowsException(long productId, decimal price)
    {
        // Act
        var act = () => _productService.UpdateProductPrice(productId, price);
        
        // Assert
        act.Should().Throw<ProductNotFoundException>();
        _productRepositoryMock.Verify(x => x.GetById(productId), Times.Once);
    }

    [Theory, AutoData]
    public void UpdateProductPrice_ProductExists_PriceChanged(long productId, decimal newPrice)
    {
        // Arrange
        var product = new Fixture().Create<Product>();

        _productRepositoryMock
            .Setup(x => x.GetById(productId))
            .Returns(product);
        
        // Act
        var updatedProductDto = _productService.UpdateProductPrice(productId, newPrice);
        
        // Assert
        updatedProductDto.Price.Should().Be(newPrice);
        _productRepositoryMock.Verify(x => x.GetById(productId), Times.Once);
        _productRepositoryMock.Verify(x => x.Update(It.IsAny<Product>()));
    }

    [Theory, AutoData]
    public void GetFilteredProducts_NoProducts_ReturnsEmptyCollection(int page, int pageSize)
    {
        // Arrange
        _productRepositoryMock.DefaultValue = DefaultValue.Mock;
        
        // Act
        var productsDto = 
            _productService.GetFilteredProducts(page, pageSize, null, null, null);
        
        // Assert
        productsDto.Should().BeEmpty();
        _productRepositoryMock.Verify(x => x.GetProducts(), Times.Once);
    }
    
    [Theory, AutoData]
    public void GetFilteredProducts_SomeProductsExist_ReturnsProducts(IReadOnlyCollection<Product> products)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);
        const int page = 1;
        const int pageSize = 5;
        
        // Act
        var productsDto = 
            _productService.GetFilteredProducts(page, pageSize, null, null, null);
        
        // Assert
        productsDto.Should().NotBeEmpty();
        _productRepositoryMock.Verify(x => x.GetProducts(), Times.Once);
    }
    
    [Theory, AutoData]
    public void GetFilteredProducts_PageSizeZero_ReturnsEmptyCollection(IReadOnlyCollection<Product> products)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);
        const int page = 1;
        const int pageSize = 0;
        
        // Act
        var productsDto = 
            _productService.GetFilteredProducts(page, pageSize, null, null, null);
        
        // Assert
        productsDto.Should().BeEmpty();
        _productRepositoryMock.Verify(x => x.GetProducts(), Times.Once);
    }
    
    [Theory, AutoData]
    public void GetFilteredProducts_PageMoreThanProducts_ReturnsEmptyCollection(IReadOnlyCollection<Product> products)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);
        int page = products.Count;
        int pageSize = 5;
        
        // Act
        var productsDto = 
            _productService.GetFilteredProducts(page, pageSize, null, null, null);
        
        // Assert
        productsDto.Should().BeEmpty();
        _productRepositoryMock.Verify(x => x.GetProducts(), Times.Once);
    }
    
    [Theory, AutoData]
    public void GetFilteredProducts_FilterByWarehouseId_ReturnsFilteredProducts(
        IReadOnlyCollection<Product> products)
    {
        // Arrange
        _productRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);

        var someProduct = products.First();
        var warehouseCount = products.Count(x => x.WarehouseId == someProduct.WarehouseId);
        
        int page = 1;
        int pageSize = products.Count;
        
        // Act
        var productsDto = 
            _productService.GetFilteredProducts(page, pageSize, null, null, someProduct.WarehouseId.Value);
        
        // Assert
        productsDto.Should().NotBeEmpty();
        productsDto.Count.Should().Be(warehouseCount);
        _productRepositoryMock.Verify(x => x.GetProducts(), Times.Once);
    }
}