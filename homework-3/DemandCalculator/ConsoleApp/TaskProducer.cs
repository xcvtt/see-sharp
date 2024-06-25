using System.Threading.Channels;

namespace ConsoleApp;

public class TaskProducer
{
    private readonly ChannelWriter<ProductInfo> _channelWriter;
    private readonly string _filePath;
    private readonly CalculationInfo _calculationInfo;

    public TaskProducer(ChannelWriter<ProductInfo> channelWriter, string filePath, CalculationInfo calculationInfo)
    {
        ArgumentNullException.ThrowIfNull(channelWriter);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(calculationInfo);
        
        _channelWriter = channelWriter;
        _filePath = filePath;
        _calculationInfo = calculationInfo;
    }

    public async Task StartProduce()
    {
        foreach (var line in File.ReadLines(_filePath).Skip(1))
        {
            _calculationInfo.IncrementRowsRead();
            
            var strs = line.Split(',');
            
            var productId = int.Parse(strs[0]);
            var productPrediction = int.Parse(strs[1]);
            var productStock = int.Parse(strs[2]);

            var productInfo = new ProductInfo(productId, productPrediction, productStock);
            
            await _channelWriter.WriteAsync(productInfo);
        }

        _channelWriter.Complete();
    }
}