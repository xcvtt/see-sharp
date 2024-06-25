using Domain.Entities;

namespace Domain.Repository;

public interface IProductDailyHistoryRepository
{
    IReadOnlyCollection<ProductDailyHistory> GetProductDailyHistory(long productId);
}