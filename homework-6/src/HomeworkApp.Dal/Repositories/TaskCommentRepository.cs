using System.Transactions;
using Dapper;
using HomeworkApp.Dal.Entities;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.Dal.Settings;
using Microsoft.Extensions.Options;

namespace HomeworkApp.Dal.Repositories;

public class TaskCommentRepository : PgRepository, ITaskCommentRepository
{
    public TaskCommentRepository(
        IOptions<DalOptions> dalSettings) : base(dalSettings.Value)
    {
    }
    
    
    public async Task<long> Add(TaskCommentEntityV1 model, CancellationToken token)
    {
        var sqlQuery = """
                       insert into task_comments (id, task_id, author_user_id, message, at, modified_at, deleted_at) 
                       values (@Id, @TaskId, @AuthorUserId, @Message, @At, @ModifiedAt, @DeletedAt)
                       returning id
                       """;

        var cmd = new CommandDefinition(
            sqlQuery,
            model,
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token
        );
        
        await using var connection = await GetConnection();

        var id = await connection.ExecuteScalarAsync<long>(cmd);

        return id;
    }

    public async Task Update(TaskCommentEntityV1 model, CancellationToken token)
    {
        var sqlQuery = """
                       update task_comments
                       set message = @Message,
                           modified_at = @ModifiedAt
                       where id = @Id
                       """;

        var cmd = new CommandDefinition(
            sqlQuery,
            new
            {
                Message = model.Message,
                ModifiedAt = model.ModifiedAt,
                Id = model.Id,
            },
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(cmd);
    }

    public async Task SetDeleted(long taskId, CancellationToken token)
    {
        var sqlQuery = """
                       update task_comments
                       set deleted_at = @DeletedAt
                       where task_id = @TaskId
                       """;
        
        var cmd = new CommandDefinition(
            sqlQuery,
            new
            {
                DeletedAt = DateTimeOffset.UtcNow,
                TaskId = taskId,
            },
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();

        using var transaction = CreateTransactionScope();
        await connection.ExecuteAsync(cmd);
	
        transaction.Complete();
    }

    public async Task<TaskCommentEntityV1[]> Get(TaskCommentGetModel model, CancellationToken token)
    {
        var sqlQuery = $"""
                      select id, task_id, author_user_id, message, at, modified_at, deleted_at
                      from task_comments
                      where task_id = @TaskId
                      {(model.IncludeDeleted ? "" : "and deleted_at is null")}
                      order by id desc
                      """;

        var cmd = new CommandDefinition(
            sqlQuery,
            new
            {
                TaskId = model.TaskId
            },
            commandTimeout: DefaultTimeoutInSeconds,
            cancellationToken: token);
        
        await using var connection = await GetConnection();

        var comments = await connection.QueryAsync<TaskCommentEntityV1>(cmd);

        return comments.ToArray();
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