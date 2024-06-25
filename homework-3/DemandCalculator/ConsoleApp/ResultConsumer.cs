using System.Threading.Channels;

namespace ConsoleApp;

public class ResultConsumer
{
    private readonly ChannelReader<ResultInfo> _channel;
    private readonly string _filePath;
    private readonly CalculationInfo _calculationInfo;

    public ResultConsumer(ChannelReader<ResultInfo> channel, string filePath, CalculationInfo calculationInfo)
    {
        ArgumentNullException.ThrowIfNull(channel);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(calculationInfo);
        
        _channel = channel;
        _filePath = filePath;
        _calculationInfo = calculationInfo;
    }

    public async Task StartConsume()
    {
        await using var stream = new StreamWriter(_filePath);
        await stream.WriteLineAsync("id, demand");
        
        await foreach (var resultInfo in _channel.ReadAllAsync())
        {
            await stream.WriteLineAsync($"{resultInfo.Id}, {resultInfo.Demand}");
            _calculationInfo.IncrementRowsWritten();
        }
    }
}