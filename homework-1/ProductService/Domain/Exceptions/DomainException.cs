namespace Domain.Exceptions;

public class DomainException(string message) : Exception(message)
{
    public static InvalidMonthException InvalidMonth(int month)
    {
        return new InvalidMonthException($"Month was: {month}, valid range: 1-12");
    }
    
    public static InvalidSeasonalCoefficientException InvalidSeasonalCoefficient(decimal coefficient)
    {
        return new InvalidSeasonalCoefficientException($"Seasonal coefficient was: {coefficient}, valid range: 0-3");
    }

    public static ProductNotFoundException ProductNotFound(long productId)
    {
        return new ProductNotFoundException($"No product with id: {productId}");
    }

    public static InvalidProductIdException InvalidProductId(long productId)
    {
        return new InvalidProductIdException($"Product id was negative: {productId}");
    }
    
    public static InvalidProductSalesException InvalidProductSales(int productSales)
    {
        return new InvalidProductSalesException($"Product sales was negative: {productSales}");
    }
    
    public static InvalidProductStockException InvalidProductStock(int productStock)
    {
        return new InvalidProductStockException($"Product stock was negative: {productStock}");
    }
}