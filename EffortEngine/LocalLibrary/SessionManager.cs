using DataAccess.Data;
using SharedProject.Events;
using SharedProject.Models;

namespace EffortEngine.LocalLibrary;

public class SessionManager(IPomodoroSessionData pomodoroSessionData, ITaskData taskData, IProgramData programData, IEventAggregator eventAggregator) : BindableBase
{
    public bool IsSessionAlive { get; set; } = false;
    public PomodoroSession PomodoroSession { get; set; }

    private object? lastItemAdded;

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
        if (value is TaskBase task)
        {
            PomodoroSession.Tasks.FirstOrDefault(q => q.Name == task.Name).TotalWorkTime += PomodoroTimer.TotalWorkMinutes;
        }

        else if (value is Program program)
        {
            PomodoroSession.Programs.FirstOrDefault(q => q.Name == program.Name).TotalWorkTime += PomodoroTimer.TotalWorkMinutes;
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
            WorkTime = 25,
            BreakTime = 5,
            Rounds = 4,
        };
    }

    public IAsyncCommand FinishSessionCommand => new AsyncDelegateCommand(async () =>
    {
        if (IsSessionAlive)
        {
            await AddWorkTime(lastItemAdded);
        }

        IsSessionAlive = false;
        lastItemAdded = null;
        PomodoroSession.Status = PomodoroSession.SessionStatus.Completed;
        PomodoroSession.EndTime = DateTime.Now;

        await pomodoroSessionData.InsertSessionAsync(PomodoroSession);

        await UpdateDataBase();
        eventAggregator.GetEvent<SessionFinishedEvent>().Publish();
    });

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
