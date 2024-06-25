using FluentAssertions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskCommentRepositoryTests
{
    private readonly ITaskCommentRepository _repository;

    public TaskCommentRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskCommentRepository;
    }

    [Fact]
    public async Task Add_CorrectTask_TaskAddedCorrectly()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.GetSingle();

        // Act
        var id = await _repository.Add(taskComment, default);
        var taskComments = await _repository.Get(
            new TaskCommentGetModel
            {
                TaskId = taskComment.TaskId,
                IncludeDeleted = false,
            },
            default);
        var resultTaskComment = taskComments.FirstOrDefault();

        // Assert
        taskComments.Should().HaveCount(1);
        resultTaskComment.Should().NotBeNull();
        resultTaskComment.Should().BeEquivalentTo(taskComment);
        id.Should().BeGreaterOrEqualTo(0);
    }
    
    [Fact]
    public async Task Update_TaskExists_TaskUpdatedCorrectly()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.GetSingle();
        var id = await _repository.Add(taskComment, default);
        var updatedTaskComment = taskComment with { Message = "kek", ModifiedAt = DateTimeOffset.UtcNow };
        
        // Act
        await _repository.Update(updatedTaskComment, default);
        var taskComments = await _repository.Get(
            new TaskCommentGetModel
            {
                TaskId = taskComment.TaskId,
                IncludeDeleted = false,
            },
            default);
        var resultTaskComment = taskComments.FirstOrDefault();

        // Assert
        resultTaskComment.Should().NotBeNull();
        resultTaskComment.Should().BeEquivalentTo(updatedTaskComment);
    }
    
    [Fact]
    public async Task SetDeleted_TaskExists_TaskUpdatedCorrectly()
    {
        // Arrange
        var taskComment = TaskCommentEntityV1Faker.GetSingle();
        var id = await _repository.Add(taskComment, default);
        
        // Act
        await _repository.SetDeleted(taskComment.TaskId, default);
        var taskComments = await _repository.Get(
            new TaskCommentGetModel
            {
                TaskId = taskComment.TaskId,
                IncludeDeleted = true,
            },
            default);
        var resultTaskComment = taskComments.FirstOrDefault();

        // Assert
        resultTaskComment.Should().NotBeNull();
        resultTaskComment.DeletedAt.Should().NotBeNull();
        resultTaskComment.DeletedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public async Task Get_IncludeDeleted_ReturnsCorrectTaskComments()
    {
        // Arrange
        const int commentCount = 3;
        const long taskId = 666;
        
        var taskComments = TaskCommentEntityV1Faker
            .Generate(commentCount)
            .Select(x => x.WithTaskId(taskId))
            .ToArray();
        
        taskComments[commentCount-1] = taskComments[commentCount-1].WithDeletedAt(DateTimeOffset.UtcNow);

        foreach (var comment in taskComments)
        {
            await _repository.Add(comment, default);
        }

        // Act
        var resultTaskComments = await _repository.Get(
            new TaskCommentGetModel
            {
                TaskId = taskId,
                IncludeDeleted = true,
            },
            default);

        // Assert
        resultTaskComments.Should().HaveCount(commentCount);
        resultTaskComments.Should().BeEquivalentTo(taskComments);
        resultTaskComments.Should().ContainSingle(x => x.DeletedAt != null);
    }
}