namespace SharedProject.Models;

public class TaskBase : BindableBase
{
    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Paused
    }

    public enum TaskType
    {
        Module,
        Feature,
        Bug,
        LifeTask,
        StockMarketTask,
        SystemTask
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public TaskStatus Status { get; set; }
    public TaskType Type { get; set; }
    public decimal WorkTime { get; set; }
    public string? Note { get; set; }
    public int? ProgramId { get; set; }

    public string StatusString => Status.ToString();

    public string TypeString => Type.ToString();

    public string PriorityString
    {
        get
        {
            return Priority switch
            {
                0 => "Niski",
                1 => "Średni",
                2 => "Wysoki",
                _ => "Brak"
            };
        }
    }
}
