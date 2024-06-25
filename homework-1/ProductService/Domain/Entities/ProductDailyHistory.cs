using Domain.ValueObjects;

namespace Domain.Entities;

public sealed class ProductDailyHistory
{
    public ProductId ProductId { get; }
    public DateOnly Day { get; }
    public ProductSales Sales { get; }
    public ProductStock Stock { get; }

    public ProductDailyHistory(ProductId productId, DateOnly day, ProductSales sales, ProductStock stock)
    {
        ArgumentNullException.ThrowIfNull(productId);
        ArgumentNullException.ThrowIfNull(day);
        ArgumentNullException.ThrowIfNull(sales);
        ArgumentNullException.ThrowIfNull(stock);

        ProductId = productId;
        Day = day;
        Sales = sales;
        Stock = stock;
    }
}