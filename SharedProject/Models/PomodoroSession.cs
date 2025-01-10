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
    public int TaskId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int WorkTime { get; set; }
    public int BreakTime { get; set; }
    public int Rounds { get; set; }
    public SessionStatus Status { get; set; }
    public TaskBase Task { get; set; }
}