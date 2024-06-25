using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using IntegrationTests.Helpers;
using Moq;
using WebAPI;
using WebAPI.Grpc;
using Xunit.Abstractions;
using Product = Domain.Entities.Product;

namespace IntegrationTests;

public class ProductServiceGrpcTests : IClassFixture<CustomWebApplicationFactory<EntryPoint>>
{
    private readonly CustomWebApplicationFactory<EntryPoint> _webApplicationFactory;
    private readonly ITestOutputHelper _outputHelper;
    private readonly ProductGrpc.ProductGrpcClient _grpcClient;

    public ProductServiceGrpcTests(
        CustomWebApplicationFactory<EntryPoint> webApplicationFactory,
        ITestOutputHelper outputHelper)
    {
        _webApplicationFactory = webApplicationFactory;
        _outputHelper = outputHelper;

        var client = _webApplicationFactory.CreateClient();
        
        var grpcChannel = GrpcChannel.ForAddress(client.BaseAddress!, new GrpcChannelOptions
        {
            HttpClient = client
        });

        _grpcClient = new ProductGrpc.ProductGrpcClient(grpcChannel);
    }


    [Theory, AutoData]
    public async Task GetProduct_ProductExists_ReturnsCorrectProduct(Product product)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        var getProductRequest = new GetProductRequest
        {
            ProductId = product.ProductId.Value
        }; 
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetById(product.ProductId.Value))
            .Returns(product);
        
        // Act
        var response = await _grpcClient.GetProductAsync(getProductRequest);
        
        // Assert
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(product.ProductId.Value), Times.Once);
        response.Should().NotBeNull();
        response.Product.ProductId.Should().Be(product.ProductId.Value);
        response.Product.ProductName.Should().Be(product.ProductName.Value);
        response.Product.WarehouseId.Should().Be(product.WarehouseId.Value);
    }
    
    [Theory, AutoData]
    public async Task GetProduct_ProductDoesntExist_ReturnsInternalError(GetProductRequest request)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        // Act
        var act = async () => await _grpcClient.GetProductAsync(request);
        
        // Assert
        var exception = await act.Should().ThrowAsync<RpcException>();
        exception.And.Message.Should().Contain("Exception was thrown by handler");
    }

    
    [Theory, AutoData]
    public async Task CreateProduct_ValidData_ProductCreated(AddProductRequest request)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        // Act
        var response = await _grpcClient.AddProductAsync(request);

        // Assert
        response.Should().NotBeNull();
        request.ProductName.Should().Be(response.Product.ProductName);
        request.WarehouseId.Should().Be(response.Product.WarehouseId);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(response.Product.ProductId), Times.Once);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
    }
    
    [Theory, AutoData]
    public async Task CreateProduct_InvalidData_ReturnsInternalError(AddProductRequest request)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        request.WarehouseId = -777;
        request.ProductWeight = -777;
        
        // Act
        var act = async () => await _grpcClient.AddProductAsync(request);

        // Assert
        var exception = await act.Should().ThrowAsync<RpcException>();
        exception.And.Message.Should().Contain("Exception was thrown by handler");
    }

    
    [Theory, AutoData]
    public async Task UpdateProductPrice_ValidData_ProductPriceUpdated(
        UpdateProductPriceRequest request,
        Product product)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetById(product.ProductId.Value))
            .Returns(product);

        request.ProductId = product.ProductId.Value;
        
        // Act
        var response = await _grpcClient.UpdateProductPriceAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Product.ProductPrice.Should().Be(request.ProductPrice);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(response.Product.ProductId), Times.Once);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task GetFilteredProducts_PageFilter_ReturnsCorrectProducts()
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        const int page = 1;
        const int pageSize = 55;
        
        var products = new Fixture()
            .CreateMany<Product>(pageSize)
            .ToList();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);

        var request = new GetFilteredProductsRequest
        {
            Page = page,
            PageSize = pageSize
        };
        
        // Act
        var response = await _grpcClient.GetFilteredProductsAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Products.Should().NotBeEmpty();
        response.Products.Count.Should().Be(products.Count);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetProducts(), Times.Once);
    }
    
    [Fact]
    public async Task GetFilteredProducts_ApplyWarehouseFilter_ReturnsCorrectProducts()
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        const int page = 1;
        const int pageSize = 300;
        
        var products = new Fixture()
            .CreateMany<Product>(pageSize)
            .ToList();

        var warehouseId = products.First().WarehouseId.Value;
        var filteredCount = products.Count(x => x.WarehouseId.Value == warehouseId);
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);
        
        var request = new GetFilteredProductsRequest
        {
            Page = page,
            PageSize = pageSize,
            WarehouseId = warehouseId
        };
        
        // Act
        var response = await _grpcClient.GetFilteredProductsAsync(request);

        // Assert
        response.Should().NotBeNull();
        response.Products.Should().NotBeEmpty();
        response.Products.Count.Should().Be(filteredCount);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetProducts(), Times.Once);
    }
}