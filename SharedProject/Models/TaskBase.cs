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
    public decimal TotalWorkTime { get; set; }
    public string? Note { get; set; }
    public int? ProgramId { get; set; }

    public string StatusString
    {
        get
        {
            return Status switch
            {
                (TaskStatus)0 => "Dodane",
                (TaskStatus)1 => "W trakcie",
                (TaskStatus)2 => "Zakończone",
                (TaskStatus)3 => "Wstrzymane",
            };
        }
    }

    public string TypeString
    {
        get
        {
            return Type switch
            {
                (TaskType)0 => "Moduł",
                (TaskType)1 => "Funkcja",
                (TaskType)2 => "Bug",
                (TaskType)3 => "Życie",
                (TaskType)4 => "Giełda",
                (TaskType)5 => "System",
            };
        }
    }

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
