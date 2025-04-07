namespace SharedProject.Models;

public class PomodoroConfig
{
    public int WorkDurationMinutes { get; set; } = 30;
    public int BreakDurationMinutes { get; set; } = 10;
    public int RoundsCount { get; set; } = 2;
}
