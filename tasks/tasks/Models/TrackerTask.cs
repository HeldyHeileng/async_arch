namespace tasks.Models;

public class TrackerTask
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string? Description { get; set; }
    public Guid AccountId { get; set; }
    public TaskStatusEnum Status { get; set; }

}

public enum TaskStatusEnum
{
    Active,
    Completed,
}