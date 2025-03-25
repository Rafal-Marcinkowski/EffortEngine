using SharedProject.Events;

namespace EffortEngine.LocalLibrary;

public class WorkManager : BindableBase
{
    public WorkManager(SessionManager sessionManager, IEventAggregator eventAggregator, PomodoroTimer pomodoroTimer, TaskManager taskManager)
    {
        this.SessionManager = sessionManager;
        this.eventAggregator = eventAggregator;
        PomodoroTimer = pomodoroTimer;
        this.taskManager = taskManager;

        this.eventAggregator.GetEvent<StartWorkEvent>().Subscribe(async taskName => await OnNewTaskStarted(taskName));
    }

    public PomodoroTimer PomodoroTimer { get; }

    private readonly TaskManager taskManager;
    public SessionManager SessionManager { get; set; }
    private readonly IEventAggregator eventAggregator;

    public async Task OnNewTaskStarted(string taskName)
    {
        if (!SessionManager.IsSessionAlive)
        {
            await SessionManager.StartSession();
        }

        PomodoroTimer.CurrentTaskText = "Praca nad: " + taskName;
        PomodoroTimer.RoundCounter = $"Runda: 1/{SessionManager.PomodoroSession.Rounds}";

        await taskManager.EvaluateTask(taskName);
        await SessionManager.AddTaskToSession(TaskManager.CurrentTask is null ? TaskManager.CurrentProgram : TaskManager.CurrentTask);
    }
}
