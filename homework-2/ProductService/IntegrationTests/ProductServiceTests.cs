using System.Net;
using System.Text;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using IntegrationTests.Helpers;
using Moq;
using WebAPI;
using WebAPI.Grpc;
using Xunit.Abstractions;
using Product = Domain.Entities.Product;

namespace IntegrationTests;

public class ProductServiceTests : IClassFixture<CustomWebApplicationFactory<EntryPoint>>
{
    private readonly CustomWebApplicationFactory<EntryPoint> _webApplicationFactory;
    private readonly ITestOutputHelper _outputHelper;
    private readonly HttpClient _client;
    private const string BaseApiPath = "v1/products";

    public ProductServiceTests(
        CustomWebApplicationFactory<EntryPoint> webApplicationFactory,
        ITestOutputHelper outputHelper)
    {
        _webApplicationFactory = webApplicationFactory;
        _outputHelper = outputHelper;
        _client = _webApplicationFactory.CreateClient();
    }


    [Theory, AutoData]
    public async Task GetProduct_ProductExists_ReturnsCorrectProduct(Product product)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetById(product.ProductId.Value))
            .Returns(product);
        
        var apiGetPath = $"{BaseApiPath}/{product.ProductId.Value}";
        
        // Act
        var response = await _client.GetAsync(apiGetPath);
        var json = await response.Content.ReadAsStringAsync();
        var result = GetProductResponse.Parser.ParseJson(json);
        
        // Assert
        response.EnsureSuccessStatusCode();
        result.Should().NotBeNull();
        result.Product.ProductId.Should().Be(product.ProductId.Value);
            
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(product.ProductId.Value), Times.Once);
    }
    
    [Theory, AutoData]
    public async Task GetProduct_ProductDoesntExist_ReturnsInternalError(Product product)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        const int productId = 777;
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetById(product.ProductId.Value))
            .Returns(product);
        
        var apiGetPath = $"{BaseApiPath}/{productId}";
        
        // Act
        var response = await _client.GetAsync(apiGetPath);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(productId), Times.Once);
    }

    [Theory, AutoData]
    public async Task CreateProduct_ValidData_ProductCreated(AddProductRequest request)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        const string apiPostPath = BaseApiPath;
        
        var httpRequest = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync(apiPostPath, httpRequest);
        var json = await response.Content.ReadAsStringAsync();
        var result = AddProductResponse.Parser.ParseJson(json);

        // Assert
        response.EnsureSuccessStatusCode();

        result.Should().NotBeNull();
        result.Product.ProductName.Should().Be(request.ProductName);
        result.Product.CreatedDate.ToDateTime().Should().Be(request.CreatedDate.ToDateTime());
        result.Product.ProductPrice.Should().Be(request.ProductPrice);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(result.Product.ProductId), Times.Once);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.Add(It.IsAny<Product>()), Times.Once);
    }
    
    [Theory, AutoData]
    public async Task CreateProduct_InvalidData_ReturnsInternalError(AddProductRequest request)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        const string apiPostPath = BaseApiPath;

        request.WarehouseId = -777;
        request.ProductWeight = -777;
        
        var httpRequest = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PostAsync(apiPostPath, httpRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(It.IsAny<long>()), Times.Never);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.Add(It.IsAny<Product>()), Times.Never);
    }

    [Theory, AutoData]
    public async Task UpdateProductPrice_ValidData_ProductPriceUpdated(
        UpdateProductPriceRequest request,
        Product product)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        var apiPutPath = $"{BaseApiPath}/{product.ProductId.Value}";
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetById(product.ProductId.Value))
            .Returns(product);

        request.ProductId = product.ProductId.Value;
        
        var httpRequest = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
        
        // Act
        var response = await _client.PutAsync(apiPutPath, httpRequest);
        var json = await response.Content.ReadAsStringAsync();
        var result = UpdateProductResponse.Parser.ParseJson(json);

        // Assert
        response.EnsureSuccessStatusCode();

        result.Should().NotBeNull();
        result.Product.ProductPrice.Should().Be(request.ProductPrice);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetById(request.ProductId), Times.Once);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
    }

    [Theory, AutoData]
    public async Task GetFilteredProducts_PageFilter_ReturnsCorrectProducts(List<Product> products)
    {
        // Arrange
        _webApplicationFactory.ProductRepositoryMock.Reset();
        
        const int page = 1;
        const int pageSize = 55;
        
        var apiGetPath = $"{BaseApiPath}?" +
                         $"page={page}&" +
                         $"pageSize={pageSize}";
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);
        
        
        // Act
        var response = await _client.GetAsync(apiGetPath);
        var json = await response.Content.ReadAsStringAsync();
        var result = GetFilteredProductsResponse.Parser.ParseJson(json);

        // Assert
        response.EnsureSuccessStatusCode();

        result.Should().NotBeNull();
        result.Products.Should().NotBeEmpty();
        result.Products.Count.Should().Be(products.Count);
        
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
        
        var apiGetPath = $"{BaseApiPath}?" +
                          $"page={page}&" +
                          $"pageSize={pageSize}&" +
                          $"warehouseId={warehouseId}";
        
        
        _webApplicationFactory.ProductRepositoryMock
            .Setup(x => x.GetProducts())
            .Returns(products);
        
        
        // Act
        var response = await _client.GetAsync(apiGetPath);
        var json = await response.Content.ReadAsStringAsync();
        var result = GetFilteredProductsResponse.Parser.ParseJson(json);

        // Assert
        response.EnsureSuccessStatusCode();
        
        result.Should().NotBeNull();
        result.Products.Should().NotBeEmpty();
        result.Products.Count.Should().Be(filteredCount);
        
        _webApplicationFactory.ProductRepositoryMock
            .Verify(x => x.GetProducts(), Times.Once);
    }
}