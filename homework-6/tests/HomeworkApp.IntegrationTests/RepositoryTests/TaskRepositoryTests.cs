using FluentAssertions;
using HomeworkApp.Dal.Models;
using HomeworkApp.Dal.Repositories.Interfaces;
using HomeworkApp.IntegrationTests.Creators;
using HomeworkApp.IntegrationTests.Fakers;
using HomeworkApp.IntegrationTests.Fixtures;
using Xunit;
using TaskStatus = HomeworkApp.Dal.Enums.TaskStatus;

namespace HomeworkApp.IntegrationTests.RepositoryTests;

[Collection(nameof(TestFixture))]
public class TaskRepositoryTests
{
    private readonly ITaskRepository _repository;

    public TaskRepositoryTests(TestFixture fixture)
    {
        _repository = fixture.TaskRepository;
    }

    [Fact]
    public async Task Add_Task_Success()
    {
        // Arrange
        const int count = 5;

        var tasks = TaskEntityV1Faker.Generate(count);
        
        // Act
        var results = await _repository.Add(tasks, default);

        // Asserts
        results.Should().HaveCount(count);
        results.Should().OnlyContain(x => x > 0);
    }
    
    [Fact]
    public async Task Get_SingleTask_Success()
    {
        // Arrange
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId);
        
        // Act
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        // Asserts
        results.Should().HaveCount(1);
        var task = results.Single();

        task.Should().BeEquivalentTo(expectedTask);
    }
    
    [Fact]
    public async Task AssignTask_Success()
    {
        // Arrange
        var assigneeUserId = Create.RandomId();
        
        var tasks = TaskEntityV1Faker.Generate();
        var taskIds = await _repository.Add(tasks, default);
        var expectedTaskId = taskIds.First();
        var expectedTask = tasks.First()
            .WithId(expectedTaskId)
            .WithAssignedToUserId(assigneeUserId);
        var assign = AssignTaskModelFaker.Generate()
            .First()
            .WithTaskId(expectedTaskId)
            .WithAssignToUserId(assigneeUserId);
        
        // Act
        await _repository.Assign(assign, default);
        
        // Asserts
        var results = await _repository.Get(new TaskGetModel()
        {
            TaskIds = new[] { expectedTaskId }
        }, default);
        
        results.Should().HaveCount(1);
        var task = results.Single();
        
        expectedTask = expectedTask with {Status = assign.Status};
        task.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async Task GetSubTasksInStatus_SubTasksExist_ReturnsAllSubTasks()
    {
        // Arrange
        const int taskCount = 6;
        const TaskStatus taskStatus = TaskStatus.InProgress;
        
        var tasks = TaskEntityV1Faker
            .Generate(taskCount)
            .Select(x => x with { Status = (int)taskStatus })
            .ToArray();
        
        var taskIds = await _repository.Add(tasks, default);
        await _repository.SetParentTask(new SetParentTaskModel()
        {  
            TaskId = taskIds[0],
            ParentTaskId = taskIds[5],
        }, default);
        
        await _repository.SetParentTask(new SetParentTaskModel()
        {  
            TaskId = taskIds[1],
            ParentTaskId = taskIds[5],
        }, default);
        
        await _repository.SetParentTask(new SetParentTaskModel()
        {  
            TaskId = taskIds[2],
            ParentTaskId = taskIds[5],
        }, default);
        
        await _repository.SetParentTask(new SetParentTaskModel()
        {  
            TaskId = taskIds[3],
            ParentTaskId = taskIds[0],
        }, default);
        

        // Act
        var subTasks = await _repository.GetSubTasksInStatus(
            taskIds[5],
            new[] { (TaskStatus)taskStatus },
            default);

        var deepestChild = subTasks.First(x => x.TaskId == taskIds[3]);

        // Assert
        subTasks.Length.Should().Be(taskCount - 2);
        subTasks.Count(x => x.Status == taskStatus).Should().Be(taskCount - 2);
        subTasks.Should().NotContain(x => x.TaskId == taskIds[5]);
        deepestChild.ParentTaskIds.Should().ContainInOrder(taskIds[5], taskIds[0], taskIds[3]);
    }
}
