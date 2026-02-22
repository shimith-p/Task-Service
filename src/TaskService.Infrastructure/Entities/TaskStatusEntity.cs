namespace TaskService.Domain.Entities;
 
public sealed class TaskStatusEntity
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

 
    private TaskStatusEntity() { }

    public static TaskStatusEntity Create(int id, string name) => new()
    {
        Id = id,
        Name = name
    };
}