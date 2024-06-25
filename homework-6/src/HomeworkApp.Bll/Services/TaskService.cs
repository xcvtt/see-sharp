using System.Text.Json;
using System.Transactions;
using HomeworkApp.Bll.Models;
using HomeworkApp.Bll.Services.Interfaces;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using AssignTaskModel = HomeworkApp.Dal.Models.AssignTaskModel;
using TaskStatus = HomeworkApp.Bll.Enums.TaskStatus;

namespace HomeworkApp.Bll.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ITaskCommentRepository _taskCommentRepository;
    private readonly ITaskLogRepository _taskLogRepository;
    private readonly ITakenTaskRepository _takenTaskRepository;
    private readonly IDistributedCache _distributedCache;

    public TaskService(
        ITaskRepository taskRepository,
        ITaskLogRepository taskLogRepository,
        ITakenTaskRepository takenTaskRepository, 
        IDistributedCache distributedCache,
        ITaskCommentRepository taskCommentRepository)
    {
        _taskRepository = taskRepository;
        _taskLogRepository = taskLogRepository;
        _takenTaskRepository = takenTaskRepository;
        _distributedCache = distributedCache;
        _taskCommentRepository = taskCommentRepository;
    }
    
    public async Task<long> CreateTask(
        CreateTaskModel model, 
        CancellationToken token)
    {
        using var transaction = CreateTransactionScope();

        var task =  new TaskEntityV1
        {
            Title = model.Title,
            Description = model.Description,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedByUserId = model.UserId,
            Status = (int) Enums.TaskStatus.Draft,
            Number = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}"
        };
        var taskId = (await _taskRepository.Add(
                new[] { task }, 
                token))
            .Single();
        
        var taskLog = new TaskLogEntityV1
        {
            TaskId = taskId,
            Number = task.Number,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            At = task.CreatedAt,
            UserId = model.UserId
        };;
        await _taskLogRepository.Add(new[] { taskLog }, token);
        
        transaction.Complete();

        return taskId;
    }
    
    public async Task<GetTaskModel?> GetTask(
        long taskId, 
        CancellationToken token)
    {
        var cacheKey = $"cache_tasks:{taskId}";
        var cachedTask = await _distributedCache.GetStringAsync(cacheKey, token);
        if (!string.IsNullOrEmpty(cachedTask))
        {
            return JsonSerializer.Deserialize<GetTaskModel>(cachedTask);
        }
        
        var task = (await _taskRepository.Get(new TaskGetModel
            {
                TaskIds = new[] {taskId}
            }, token))
            .SingleOrDefault();

        if (task is null)
        {
            return null;
        }
        
        var result = new GetTaskModel
        {
            TaskId = task.Id,
            Number = task.Number,
            AssignedToUserId = task.AssignedToUserId,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            CreatedByUserId = task.CreatedByUserId,
            Description = task.Description,
            ParentTaskId = task.ParentTaskId,
            Status = (TaskStatus)task.Status,
            Title = task.Title
        };

        var taskJson = JsonSerializer.Serialize(result);
        await _distributedCache.SetStringAsync(
            cacheKey, 
            taskJson,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            },
            token);
        
        return result;
    }

    public async Task AssignTask(
        Bll.Models.AssignTaskModel model,
        CancellationToken token)
    {
        var task = (await _taskRepository.Get(new TaskGetModel
            {
                TaskIds = new[] {model.TaskId}
            }, token))
            .SingleOrDefault();

        if (task is null)
        {
            return;
        }
        
        using var transaction = CreateTransactionScope();
        
        await _taskRepository.Assign(
            new AssignTaskModel()
            {
                TaskId = model.TaskId,
                AssignToUserId = model.AssignToUserId,
                Status = (int)Enums.TaskStatus.InProgress
            },
            token);
        
        task = task with
        {
            Status = (int)Enums.TaskStatus.InProgress,
            AssignedToUserId = model.AssignToUserId
        };

        var taskLog = new TaskLogEntityV1
        {
            TaskId = task.Id,
            Number = task.Number,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            AssignedToUserId = task.AssignedToUserId.Value,
            At = DateTimeOffset.UtcNow,
            UserId = model.UserId,
        };
        await _taskLogRepository.Add(new[] { taskLog }, token);

        await _takenTaskRepository.Add(new TakenTaskModel()
        {
            TaskId = task.Id,
            Title = task.Title,
            AssignedToUserId = task.AssignedToUserId.Value,
            AssignedAt = taskLog.At
        }, token);
        
        transaction.Complete();
    }

    public async Task<TaskMessage[]> GetComments(long taskId, CancellationToken token)
    {
        const int getLatestCount = 5;
        const int cacheKeyExpirationInSeconds = 5;
        
        var cacheKey = $"cache_task_comments:{taskId}";
        var cachedTaskComment = await _distributedCache.GetStringAsync(cacheKey, token);
        
        if (!string.IsNullOrEmpty(cachedTaskComment))
        {
            return JsonSerializer.Deserialize<TaskMessage[]>(cachedTaskComment) ?? Array.Empty<TaskMessage>();
        }
        
        var comments = await _taskCommentRepository.Get(
            new TaskCommentGetModel
            {
                TaskId = taskId,
                IncludeDeleted = true,
            },
            token);

        var messages = comments
            .Select(x => new TaskMessage
            {
                TaskId = x.TaskId,
                Comment = x.Message,
                At = x.At,
                IsDeleted = x.DeletedAt is not null,
            })
            .Take(getLatestCount)
            .ToArray();
        
        var messagesJson = JsonSerializer.Serialize(messages);
        await _distributedCache.SetStringAsync(
            cacheKey, 
            messagesJson,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheKeyExpirationInSeconds),
            },
            token);

        return messages;
    }

    private TransactionScope CreateTransactionScope(
        IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions 
            { 
                IsolationLevel = level, 
                Timeout = TimeSpan.FromSeconds(5) 
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}