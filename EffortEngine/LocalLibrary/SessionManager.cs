using DataAccess.Data;
using SharedProject.Events;
using SharedProject.Models;

namespace EffortEngine.LocalLibrary;

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
            PomodoroSession.Tasks.Last().TotalWorkTime += PomodoroTimer.TotalWorkMinutes;
            eventAggregator.GetEvent<WorkTimeAddedEvent>().Publish(PomodoroSession.Tasks.Last().Id);
        }

        else if (value is Program)
        {
            PomodoroSession.Programs.Last().TotalWorkTime += PomodoroTimer.TotalWorkMinutes;
            eventAggregator.GetEvent<WorkTimeAddedEvent>().Publish(PomodoroSession.Programs.Last().Id);
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
            RoundWorkTime = 25,
            TotalWorkTime = 0,
            BreakTime = 5,
            Rounds = 4,
        };
    }

    public IAsyncCommand FinishSessionCommand => new AsyncDelegateCommand(async () =>
    {
        if (IsSessionAlive)
        {
            await AddWorkTime(lastItemAdded);

            IsSessionAlive = false;
            lastItemAdded = null;

            PomodoroSession.TotalWorkTime += PomodoroTimer.ActiveWorkMinutes;

            if (PomodoroSession.TotalWorkTime >= 1)
            {
                PomodoroSession.Status = PomodoroSession.SessionStatus.Completed;
                PomodoroSession.EndTime = DateTime.Now;

                await pomodoroSessionData.InsertSessionAsync(PomodoroSession);

                await UpdateDataBase();
            }

            eventAggregator.GetEvent<SessionFinishedEvent>().Publish();
        }
    });

    public async Task HandleSessionElapsed()
    {
        if (IsSessionAlive)
        {
            await AddWorkTime(lastItemAdded);
            PomodoroSession.TotalWorkTime += PomodoroTimer.ActiveWorkMinutes;
            eventAggregator.GetEvent<SessionFinishedEvent>().Publish();
        }
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
