using DataAccess.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;

namespace DataAccess.Parser;

public static class FileParser
{
    public static List<ProductDailyHistory> ParseDailyHistory(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw DataAccessException.WrongFilePath(filePath);
        }

        var productsHistoryLines = File.ReadAllLines(filePath).Skip(1).ToList();
        var productsDailyHistory = new List<ProductDailyHistory>(productsHistoryLines.Count);

        foreach (var line in productsHistoryLines)
        {
            var data = line.Split(',');
            
            var productId = int.Parse(data[0]);
            var date = DateOnly.Parse(data[1]);
            var sales = int.Parse(data[2]);
            var stock = int.Parse(data[3]);
            
            productsDailyHistory.Add(new ProductDailyHistory(
                new ProductId(productId),
                date,
                new ProductSales(sales),
                new ProductStock(stock)));
        }

        return productsDailyHistory;
    }

    public static List<ProductSeasonalCoefficient> ParseSeasonalCoefficients(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        if (!File.Exists(filePath))
        {
            throw DataAccessException.WrongFilePath(filePath);
        }

        var coeffsLines = File.ReadAllLines(filePath).Skip(1).ToList();
        var coeffsHistory = new List<ProductSeasonalCoefficient>(coeffsLines.Count);

        foreach (var line in coeffsLines)
        {
            var data = line.Split(',');

            var productId = int.Parse(data[0]);
            var month = int.Parse(data[1]);
            var coef = decimal.Parse(data[2]);
            
            coeffsHistory.Add(new ProductSeasonalCoefficient(
                new ProductId(productId),
                new Month(month),
                new SeasonalCoefficient(coef)));
        }

        return coeffsHistory;
    }
}