using EffortEngine.LocalLibrary;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Views;

namespace EffortEngine.MVVM.ViewModels;

public class ManageTasksViewModel : BindableBase, INavigationAware
{
    public ManageTasksViewModel(IEventAggregator eventAggregator, ViewManager viewManager)
    {
        this.eventAggregator = eventAggregator;
        this.viewManager = viewManager;
        this.TaskTableViewModel = ContainerLocator.Container.Resolve<TaskTableViewModel>();
        this.eventAggregator.GetEvent<WorkTimeAddedEvent>().Subscribe(async (id) => await TaskTableViewModel.RefreshUI(id));
    }

    public TaskTableViewModel TaskTableViewModel { get; set; }
    private readonly IEventAggregator eventAggregator;
    private readonly ViewManager viewManager;

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        TaskTableViewModel.Clear();
        ShowAllTasksCommand.ExecuteAsync(null);
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return false;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {

    }

    public TaskBase SelectedTask => TaskTableViewModel.SelectedTask;
    public Program SelectedProgram => TaskTableViewModel.SelectedProgram;

    public IAsyncCommand ShowProgramsCommand => new AsyncDelegateCommand(async () => await TaskTableViewModel.ShowPrograms());

    public IAsyncCommand ShowAllTasksCommand => new AsyncDelegateCommand(async () => await TaskTableViewModel.ShowAllTasks());

    public IAsyncCommand ShowAllProgrammingTasksCommand => new AsyncDelegateCommand(async () => await TaskTableViewModel.ShowAllProgrammingTasks());

    public IAsyncCommand ShowSystemTasksCommand => new AsyncDelegateCommand(async () => await TaskTableViewModel.ShowSystemTasks());

    public IAsyncCommand ShowStockMarketTasksCommand => new AsyncDelegateCommand(async () => await TaskTableViewModel.ShowStockMarketTasks());

    public IAsyncCommand ShowLifeTasksCommand => new AsyncDelegateCommand(async () => await TaskTableViewModel.ShowLifeTasks());

    public IAsyncCommand StartWorkCommand => new AsyncDelegateCommand(async () =>
    {
        if (!String.IsNullOrEmpty(TaskTableViewModel.SelectedTaskName) && TaskTableViewModel.SelectedTaskStatus != nameof(TaskBase.TaskStatus.Completed))
        {
            var dialog = new ConfirmationDialog { DialogText = $"Rozpocząć pracę nad: {TaskTableViewModel.SelectedTaskName}?" };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                eventAggregator.GetEvent<StartWorkEvent>().Publish(TaskTableViewModel.SelectedTaskName);
            }
        }
    });

    public IAsyncCommand CompleteTaskCommand => new AsyncDelegateCommand<TaskBase>(async task =>
    {
        if (task != null)
        {
            var dialog = new ConfirmationDialog { DialogText = $"Zakończyć zadanie: {TaskTableViewModel.SelectedTaskName}?" };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                if (TaskManager.CurrentTask?.Id == task.Id && SessionManager.IsSessionAlive)
                {
                    task.TotalWorkTime += PomodoroTimer.ActiveWorkMinutes;
                    PomodoroTimer.ResetWorkTime();
                }

                task.Status = TaskBase.TaskStatus.Completed;
                task.LastUpdated = DateTime.Now;

                await viewManager.UpdateTaskAsync(task);
            }
        }
    });
}
