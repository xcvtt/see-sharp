namespace Domain.Services;

public interface ICalculateSalesPrediction
{
    decimal CalculateSalesPrediction(long productId, int days);
}