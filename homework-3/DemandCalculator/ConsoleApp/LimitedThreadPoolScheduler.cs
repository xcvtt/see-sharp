using System.Collections.Concurrent;

namespace ConsoleApp;

public class LimitedThreadPoolScheduler : TaskScheduler, IDisposable
{
    private readonly ConcurrentQueue<Task> _tasks = [];
    private int _maxThreads;
    private int _runningThreads;
    
    public LimitedThreadPoolScheduler(int maxThreads)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maxThreads, 1);
        
        _maxThreads = maxThreads;
    }

    protected override void QueueTask(Task task)
    {
        _tasks.Enqueue(task);
        
        if (_runningThreads < _maxThreads)
            NotifyThreadPool();
    }

    private void NotifyThreadPool()
    {
        Interlocked.Increment(ref _runningThreads);
        
        ThreadPool.QueueUserWorkItem(_ =>
        {
            while (_runningThreads <= _maxThreads)
            {
                if (!_tasks.TryDequeue(out var task))
                    break;

                TryExecuteTask(task);
            }

            Interlocked.Decrement(ref _runningThreads);
        });
    }

    public void ChangeMaxThreads(int threadCount)
    {
        _maxThreads = threadCount;
            
        while (_runningThreads < _maxThreads)
            NotifyThreadPool();
    }
    
    public override int MaximumConcurrencyLevel => _maxThreads;
    protected override IEnumerable<Task> GetScheduledTasks() => _tasks;
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

    public void Dispose()
    {
        _maxThreads = 0;

        while (_runningThreads != 0)
        {
        }
        
        Console.WriteLine("TaskScheduler gracefully shutdown");
    }
}