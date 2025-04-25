using DataAccess.Data;
using SharedProject.Events;
using SharedProject.Models;

namespace EffortEngine.LocalLibrary.Services;

public class SessionManager : BindableBase
{
    public static bool IsSessionAlive { get; set; } = false;
    public PomodoroSession PomodoroSession { get; set; }

    private object? lastItemAdded = null;
    private readonly IPomodoroSessionData pomodoroSessionData;
    private readonly ITaskData taskData;
    private readonly IProgramData programData;
    private readonly IEventAggregator eventAggregator;

    public SessionManager(IPomodoroSessionData pomodoroSessionData, ITaskData taskData, IProgramData programData, IEventAggregator eventAggregator)
    {
        this.pomodoroSessionData = pomodoroSessionData;
        this.taskData = taskData;
        this.programData = programData;

        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<SessionElapsedEvent>().Subscribe(async () => await HandleSessionElapsed());
    }

    public async Task AddTaskToSession(object value)
    {
        if (lastItemAdded != null)
        {
            await AddWorkTime(lastItemAdded);
        }

        if (value is TaskBase task)
        {
            PomodoroSession.Tasks.Add(task);
        }

        else
        {
            PomodoroSession.Programs.Add((Program)value);
        }

        lastItemAdded = value;
    }

    private async Task AddWorkTime(object value)
    {
        if (value is TaskBase)
        {
            PomodoroSession.Tasks[^1].TotalWorkTime += PomodoroTimer.TotalWorkMinutes;
            eventAggregator.GetEvent<WorkTimeAddedEvent>().Publish(new WorkTimeAddedEventArgs(PomodoroSession.Tasks[^1].Id,
                PomodoroTimer.TotalWorkMinutes));
        }

        else if (value is Program)
        {
            PomodoroSession.Programs[^1].TotalWorkTime += PomodoroTimer.TotalWorkMinutes;
            eventAggregator.GetEvent<WorkTimeAddedEvent>().Publish(new WorkTimeAddedEventArgs(PomodoroSession.Programs[^1].Id,
                PomodoroTimer.TotalWorkMinutes,
                true));
        }

        PomodoroTimer.ResetWorkTime();
    }

    public async Task StartSession()
    {
        IsSessionAlive = true;

        PomodoroSession = new PomodoroSession()
        {
            Status = PomodoroSession.SessionStatus.InProgress,
            StartTime = DateTime.Now,
            RoundWorkTime = 30,
            TotalWorkTime = 0,
            BreakTime = 10,
            Rounds = 2,
        };
    }

    public IAsyncCommand FinishSessionCommand => new AsyncDelegateCommand(async () =>
    {
        if (!IsSessionAlive) return;

        await AddWorkTime(lastItemAdded);

        PomodoroSession.TotalWorkTime += PomodoroTimer.ActiveWorkMinutes;

        if (PomodoroSession.TotalWorkTime >= 1)
        {
            PomodoroSession.Status = PomodoroSession.SessionStatus.Completed;
            PomodoroSession.EndTime = DateTime.Now;

            await pomodoroSessionData.InsertSessionAsync(PomodoroSession);
        }

        await UpdateDataBase();
        IsSessionAlive = false;
        lastItemAdded = null;
        eventAggregator.GetEvent<SessionFinishedEvent>().Publish();
    });

    public async Task HandleSessionElapsed()
    {
        await FinishSessionCommand.ExecuteAsync(null);
    }

    private async Task UpdateDataBase()
    {
        foreach (var task in PomodoroSession.Tasks)
        {
            await taskData.UpdateTaskAsync(task);
        }

        foreach (var program in PomodoroSession.Programs)
        {
            await programData.UpdateProgramAsync(program);
        }
    }
}
