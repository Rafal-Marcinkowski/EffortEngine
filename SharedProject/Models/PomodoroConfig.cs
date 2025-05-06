namespace SharedProject.Models;

public class PomodoroConfig : BindableBase
{
    private int _workDurationMinutes = 30;
    public int WorkDurationMinutes
    {
        get => _workDurationMinutes;
        set => SetProperty(ref _workDurationMinutes, value);
    }

    private int _breakDurationMinutes = 10;
    public int BreakDurationMinutes
    {
        get => _breakDurationMinutes;
        set => SetProperty(ref _breakDurationMinutes, value);
    }

    private int _roundsCount = 2;
    public int RoundsCount
    {
        get => _roundsCount;
        set => SetProperty(ref _roundsCount, value);
    }
}
