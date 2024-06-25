using Application.Services;

namespace ConsoleAPI.Commands;

public class CalculateProductDemandCommand : ICommand
{
    public const string Name = "demand";
    private readonly ISalesPredictionService _predictionService;
    private readonly long _productId;
    private readonly int _days;

    public CalculateProductDemandCommand(ISalesPredictionService predictionService, long productId, int days)
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
        var productDemand = _predictionService.CalculateProductDemand(_productId, _days);
        Console.WriteLine($"{productDemand}");
    }
}