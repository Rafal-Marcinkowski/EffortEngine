using DataAccess.Data;
using SharedProject.Models;

namespace EffortEngine.LocalLibrary;

public class SessionManager(IPomodoroSessionData pomodoroSessionData) : BindableBase
{
    public bool IsSessionAlive { get; set; } = false;
    public PomodoroSession PomodoroSession { get; set; }

    public SessionManager()
    {

    }

    public async Task AddTaskToSession(object value)
    {
        if (value is TaskBase)
        {
            PomodoroSession.Tasks.Add((TaskBase)value);
        }

        else
        {
            PomodoroSession.Programs.Add(((Program)value));
        }
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
        IsSessionAlive = false;
        PomodoroSession.Status = PomodoroSession.SessionStatus.Completed;
        PomodoroSession.EndTime = DateTime.Now;

        await pomodoroSessionData.InsertSessionAsync(PomodoroSession);

        foreach (var task in PomodoroSession.Tasks)
        {
            ///zastanawiam sie jak wrzuca TaskBase/Program, czy to jakos połaczyc,
        }

    });
}
