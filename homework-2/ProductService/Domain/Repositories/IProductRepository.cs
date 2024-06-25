using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IProductRepository
{
    void Add(Product product);
    void Remove(Product product);
    Product? GetById(long productId);
    IReadOnlyCollection<Product> GetProducts();
    void Update(Product product);
}