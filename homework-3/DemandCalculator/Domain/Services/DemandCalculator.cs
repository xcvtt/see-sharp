namespace ConsoleApp;

public class DemandCalculator : IDemandCalculator
{
    private readonly int _complexity;

    public DemandCalculator(int complexity = 300)
    {
        _complexity = complexity;
    }

    public decimal Calculate(int prediction, int stock)
    {
        for (int i = 0; i < Math.Pow(_complexity, 3); i++)
        {
        }

        return Math.Max(prediction - stock, 0);
    }
}