using AutoBogus;
using Bogus;
using HomeworkApp.Dal.Entities;
using HomeworkApp.IntegrationTests.Creators;

namespace HomeworkApp.IntegrationTests.Fakers;

public static class TaskCommentEntityV1Faker
{
    private static readonly Faker<TaskCommentEntityV1> Faker = new AutoFaker<TaskCommentEntityV1>()
        .RuleFor(x => x.Id, f => f.Random.UInt())
        .RuleFor(x => x.TaskId, f => f.Random.UInt())
        .RuleFor(x => x.AuthorUserId, f => f.Random.UInt())
        .RuleFor(x => x.At, f => f.Date.RecentOffset().UtcDateTime)
        .RuleFor(x => x.Message, f => f.Lorem.Sentence())
        .RuleFor(x => x.ModifiedAt, _ => null)
        .RuleFor(x => x.DeletedAt, _ => null);
    
    public static TaskCommentEntityV1[] Generate(int count = 1)
    {
        lock (Faker)
        {
            return Faker.Generate(count).ToArray();
        }
    }

    public static TaskCommentEntityV1 GetSingle()
    {
        lock (Faker)
        {
            return Faker.Generate();
        }
    }
    
    public static TaskCommentEntityV1 WithDeletedAt(
        this TaskCommentEntityV1 src, 
        DateTimeOffset deletedAt)
        => src with { DeletedAt = deletedAt };

    public static TaskCommentEntityV1 WithTaskId(
        this TaskCommentEntityV1 src,
        long taskId)
        => src with { TaskId = taskId };
}