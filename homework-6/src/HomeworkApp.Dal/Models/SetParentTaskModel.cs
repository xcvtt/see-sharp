namespace HomeworkApp.Dal.Models;

public record SetParentTaskModel
{
    public required long TaskId { get; init; }
    public required long ParentTaskId { get; init; }
}
