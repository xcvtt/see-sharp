using System.Collections.Concurrent;
using System.Collections.Immutable;
using Domain.Entities;
using Domain.Repositories;

namespace DataAccess.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<long, Product> _products = [];
    
    public void Add(Product product)
    {
        _products.TryAdd(product.ProductId.Value, product);
    }

    public void Remove(Product product)
    {
        _products.TryRemove(product.ProductId.Value, out var value);
    }

    public Product? GetById(long productId)
    {
        _products.TryGetValue(productId, out var value);
        return value;
    }

    public IReadOnlyCollection<Product> GetProducts()
    {
        return _products.Values.ToImmutableList();
    }

    public void Update(Product product)
    {
        _products.TryGetValue(product.ProductId.Value, out var oldProduct);
        _products.TryUpdate(product.ProductId.Value, product, oldProduct);
    }
}