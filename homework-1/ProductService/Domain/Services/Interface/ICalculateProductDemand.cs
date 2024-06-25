namespace Domain.Services;

public interface ICalculateProductDemand
{
    decimal CalculateProductDemand(long productId, int days);
}