using Application.Services;
using ConsoleAPI.Commands;
using DataAccess.Constants;
using DataAccess.Exceptions;
using DataAccess.Repository;
using Domain.Exceptions;
using Domain.Services;


var appName = Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0]);

if (args.Length < 2)
{
    Console.WriteLine(
        $"Correct usage: {appName} <cmd> <id> optional: <days>{Environment.NewLine}" +
        $"ads <id> - display average daily sales{Environment.NewLine}" +
        $"prediction <id> <days> - show prediction of sales <days> ahead{Environment.NewLine}" +
        $"demand <id> <days> - show how many products to buy <days> ahead{Environment.NewLine}");
}

try
{
    var productHistoryRepo = new ProductDailyHistoryRepository(FilePathConstants.ProductDailyHistoryFilePath);
    var productSeasonalCoeffsRepo =
        new ProductSeasonalCoefficientRepository(FilePathConstants.ProductSeasonalCoeffsFilePath);

    var adsCalculator = new BasicCalculateAverageDailySales(productHistoryRepo);
    var salesCalculator =
        new BasicCalculateSalesPrediction(productHistoryRepo, productSeasonalCoeffsRepo, adsCalculator);
    var demandCalculator = new BasicCalculateProductDemand(productHistoryRepo, salesCalculator);

    var salesPredictionService = new SalesPredictionService(adsCalculator, salesCalculator, demandCalculator);

    var commandFactory = new CommandFactory(salesPredictionService);

    var cmd = commandFactory.CreateCommand(args);

    cmd.Execute();
}
catch (DomainException ex)
{
    Console.WriteLine($"Domain exception: {ex.Message}");
}
catch (DataAccessException ex)
{
    Console.WriteLine($"DataAccess exception: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}


