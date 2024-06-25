namespace Domain.Services;

public interface ICalculateAverageDailySales
{
    decimal CalculateAverageDailySales(long productId);
}