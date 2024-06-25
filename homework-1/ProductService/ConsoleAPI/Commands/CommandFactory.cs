using Application.Services;

namespace ConsoleAPI.Commands;

public class CommandFactory
{
    private readonly ISalesPredictionService _predictionService;

    public CommandFactory(ISalesPredictionService predictionService)
    {
        ArgumentNullException.ThrowIfNull(predictionService);
        
        _predictionService = predictionService;
    }

    public ICommand CreateCommand(string[] args)
    {
        var cmd = args[0];
        var productId = long.Parse(args[1]);
        
        if (cmd == CalculateAdsCommand.Name)
        {
            return new CalculateAdsCommand(_predictionService, productId);
        } 
        else if (cmd == CalculateSalesPredictionCommand.Name)
        {
            var days = int.Parse(args[2]);
            return new CalculateSalesPredictionCommand(_predictionService, productId, days);
        } 
        else if (cmd == CalculateProductDemandCommand.Name)
        {
            var days = int.Parse(args[2]);
            return new CalculateProductDemandCommand(_predictionService, productId, days);
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}