using SharedProject.Events;

namespace EffortEngine.LocalLibrary;

public class WorkManager : BindableBase
{
    public WorkManager(SessionManager sessionManager, IEventAggregator eventAggregator, PomodoroTimer pomodoroTimer, TaskManager taskManager)
    {
        this.sessionManager = sessionManager;
        this.eventAggregator = eventAggregator;
        PomodoroTimer = pomodoroTimer;
        this.taskManager = taskManager;

        this.eventAggregator.GetEvent<StartWorkEvent>().Subscribe(async taskName => await OnNewTaskStarted(taskName));
    }

    public PomodoroTimer PomodoroTimer { get; }

    private readonly TaskManager taskManager;
    private readonly SessionManager sessionManager;
    private readonly IEventAggregator eventAggregator;

    public async Task OnNewTaskStarted(string taskName)
    {
        if (!sessionManager.IsSessionAlive)
        {
            await sessionManager.StartSession();
        }

        ///sprawdzic czy wczesniej nie byl jakis inny task
        PomodoroTimer.CurrentTaskText = "Praca nad: " + taskName;
        await taskManager.EvaluateTask(taskName);
        await sessionManager.AddTaskToSession(taskManager.CurrentTask is null ? taskManager.CurrentProgram : taskManager.CurrentTask);
    }
}
