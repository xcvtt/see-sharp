using System.Threading.Channels;
using ConsoleApp.options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ConsoleApp;

public static class Program {
    public static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Correct usage: program <data.csv> <channel capacity>");
            return 1;
        }

        var filePath = args[0];
        var channelCapacity = int.Parse(args[1]);
        var resultPath = $"{filePath}.res";

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File doesn't exist {filePath}");
            return 1;
        }

        if (channelCapacity < 1)
        {
            Console.WriteLine("Channel capacity should be at least 1");
            return 1;
        }
        
        var serviceCollection = new ServiceCollection();
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config/appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        serviceCollection.Configure<ConcurrencyOptions>(config.GetSection(nameof(ConcurrencyOptions)));

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var concurrencyOptionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<ConcurrencyOptions>>();
        var concurrencyOptions = concurrencyOptionsMonitor.CurrentValue;

        var chan = Channel.CreateBounded<ProductInfo>(channelCapacity);
        var resultChan = Channel.CreateUnbounded<ResultInfo>();

        var calcInfo = new CalculationInfo();
        using var scheduler = new LimitedThreadPoolScheduler(concurrencyOptions.MaxThreads);
        
        var producer = new TaskProducer(chan, filePath, calcInfo);
        var consumer = new TaskConsumer(chan, resultChan, calcInfo, scheduler, new DemandCalculator());
        var resultConsumer = new ResultConsumer(resultChan, resultPath, calcInfo);
        
        Task.Run(() => producer.StartProduce());
        Task.Run(() => consumer.StartConsume());
        Task.Run(() => resultConsumer.StartConsume());

        var timer = new System.Timers.Timer(1000);
        timer.Elapsed += (sender, e) =>
        {
            Console.Clear();
            Console.WriteLine(calcInfo); 
            Console.WriteLine($"ThreadPool count: {ThreadPool.ThreadCount}");
            Console.WriteLine();
            Console.WriteLine("Press enter to exit...");
        };
        timer.Enabled = true;
        
        concurrencyOptionsMonitor.OnChange(x =>
        {
            scheduler.ChangeMaxThreads(x.MaxThreads);
            Console.WriteLine($"New max threads: {x.MaxThreads}");
        });
        
        Console.ReadLine();

        var mp = new SortedDictionary<char, int>();
        
        

        return 0;
    }
}