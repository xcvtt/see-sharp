namespace ConsoleApp;

public interface IDemandCalculator
{
    decimal Calculate(int prediction, int stock);
}