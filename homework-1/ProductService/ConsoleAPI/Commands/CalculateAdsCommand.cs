using Application.Services;

namespace ConsoleAPI.Commands;

public class CalculateAdsCommand : ICommand
{
    public const string Name = "ads";
    private readonly ISalesPredictionService _predictionService;
    private readonly long _productId;

    public CalculateAdsCommand(ISalesPredictionService predictionService, long productId)
    {
        ArgumentNullException.ThrowIfNull(predictionService);
        ArgumentOutOfRangeException.ThrowIfNegative(productId);

        _predictionService = predictionService;
        _productId = productId;
    }
    
    public void Execute()
    {
        var ads = _predictionService.CalculateAverageDailySales(_productId);
        Console.WriteLine($"{ads}");
    }
}