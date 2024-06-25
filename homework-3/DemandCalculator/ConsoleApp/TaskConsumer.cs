using System.Threading.Channels;

namespace ConsoleApp;

public class TaskConsumer
{
    private readonly ChannelReader<ProductInfo> _channelReader;
    private readonly ChannelWriter<ResultInfo> _resultChannel;
    private readonly IDemandCalculator _demandCalculator;
    private readonly CalculationInfo _calculationInfo;
    private readonly TaskScheduler _scheduler;

    public TaskConsumer(
        ChannelReader<ProductInfo> channelReader,
        ChannelWriter<ResultInfo> resultChannel,
        CalculationInfo calculationInfo,
        TaskScheduler scheduler,
        IDemandCalculator demandCalculator)
    {
        ArgumentNullException.ThrowIfNull(channelReader);
        ArgumentNullException.ThrowIfNull(resultChannel);
        ArgumentNullException.ThrowIfNull(calculationInfo);
        ArgumentNullException.ThrowIfNull(scheduler);
        ArgumentNullException.ThrowIfNull(demandCalculator);
        
        _channelReader = channelReader;
        _resultChannel = resultChannel;
        _calculationInfo = calculationInfo;
        _scheduler = scheduler;
        _demandCalculator = demandCalculator;
    }

    public async Task StartConsume()
    {
        var tasks = new List<Task>();
        
        await foreach (var prodInfo in _channelReader.ReadAllAsync())
        {
            tasks.Add(Task.Factory.StartNew(
                () =>
                {
                    var demand = _demandCalculator.Calculate(prodInfo.Prediction, prodInfo.Stock);
                    _calculationInfo.IncrementProductsProcessed();
                    
                    _resultChannel.TryWrite(new ResultInfo(prodInfo.Id, demand));
                },
                CancellationToken.None,
                TaskCreationOptions.None,
                _scheduler));
        }
        
        Console.WriteLine("Consumed all tasks");

        await Task.WhenAll(tasks);
        
        _resultChannel.Complete();
    }
}