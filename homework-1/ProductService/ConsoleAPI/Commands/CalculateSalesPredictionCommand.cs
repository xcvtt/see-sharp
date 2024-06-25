using Application.Services;

namespace ConsoleAPI.Commands;

public class CalculateSalesPredictionCommand : ICommand
{
    public const string Name = "prediction";
    private readonly ISalesPredictionService _predictionService;
    private readonly long _productId;
    private readonly int _days;

    public CalculateSalesPredictionCommand(ISalesPredictionService predictionService, long productId, int days)
    {
        ArgumentNullException.ThrowIfNull(predictionService);
        ArgumentOutOfRangeException.ThrowIfNegative(productId);
        ArgumentOutOfRangeException.ThrowIfNegative(days);

        _predictionService = predictionService;
        _productId = productId;
        _days = days;
    }
    
    public void Execute()
    {
        var salesPrediction = _predictionService.CalculateSalesPrediction(_productId, _days);
        Console.WriteLine($"{salesPrediction}");
    }
}