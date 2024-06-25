namespace ConsoleApp;

public class CalculationInfo
{
    private int _rowsRead;
    private int _productsProcessed;
    private int _rowsWritten;

    public void IncrementRowsRead()
    {
        Interlocked.Increment(ref _rowsRead);
    }

    public void IncrementRowsWritten()
    {
        Interlocked.Increment(ref _rowsWritten);
    }

    public void IncrementProductsProcessed()
    {
        Interlocked.Increment(ref _productsProcessed);
    }

    public override string ToString()
    {
        return $"""
                Rows read: {_rowsRead}
                Rows written: {_rowsWritten}
                Products processed: {_productsProcessed}
                """;
    }
}