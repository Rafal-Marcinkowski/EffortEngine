namespace SharedProject.Models;

public class PomodoroSession
{
    public enum SessionStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Paused,
        Cancelled,
        Interrupted
    }

    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int WorkTime { get; set; }
    public int BreakTime { get; set; }
    public int Rounds { get; set; }
    public SessionStatus Status { get; set; }
    public int? ProgramId { get; set; }
    public List<TaskBase> Tasks { get; set; } = [];
    public List<Program> Programs { get; set; } = [];
}