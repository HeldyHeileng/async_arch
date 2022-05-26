using System.ComponentModel.DataAnnotations;

namespace accounting.Models;

public class TrackerTask
{
    [Key]
    public Guid PublicId { get; set; }
    public string JiraId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public Guid AccountId { get; set; }

    public int AssignCost { get; set; }
    public int CompleteCost { get; set; }

    public DateTime CreatedAt { get; set; }

}

public enum TaskStatusEnum
{
    Active,
    Completed,
}