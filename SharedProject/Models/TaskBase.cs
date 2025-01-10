namespace SharedProject.Models;

public class TaskBase
{
    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Paused
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public TaskStatus Status { get; set; }
    public int NrOfSessions { get; set; }
    public double WorkTime { get; set; }
    public string Notes { get; set; }
}
