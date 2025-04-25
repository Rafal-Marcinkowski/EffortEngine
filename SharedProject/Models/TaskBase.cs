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
        Bug,
        Feature,
        Module,
        LifeTask,
        StockMarketTask,
        SystemTask
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }

    private DateTime _lastUpdated;
    public DateTime LastUpdated
    {
        get => _lastUpdated;
        set => SetProperty(ref _lastUpdated, value);
    }

    public DateTime? DueDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public TaskStatus Status { get; set; }
    public TaskType Type { get; set; }

    private decimal _totalWorkTime;
    public decimal TotalWorkTime
    {
        get => _totalWorkTime;
        set => SetProperty(ref _totalWorkTime, value);
    }

    public string? Note { get; set; }
    public int? ProgramId { get; set; }

    public string StatusString
    {
        get
        {
            return Status switch
            {
                0 => "Dodane",
                TaskStatus.InProgress => "W trakcie",
                TaskStatus.Completed => "Zakończone",
                TaskStatus.Paused => "Wstrzymane",
            };
        }
    }

    public string TypeString
    {
        get
        {
            return Type switch
            {
                TaskType.Bug => "Bug",
                TaskType.Feature => "Funkcja",
                TaskType.Module => "Moduł",
                TaskType.LifeTask => "Życie",
                TaskType.StockMarketTask => "Giełda",
                TaskType.SystemTask => "System",
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
