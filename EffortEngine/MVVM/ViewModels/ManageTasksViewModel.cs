using EffortEngine.LocalLibrary;
using EffortEngine.LocalLibrary.Services;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Views;

namespace EffortEngine.MVVM.ViewModels;

public class ManageTasksViewModel(IEventAggregator eventAggregator, DataCoordinator dataCoordinator, TaskTableViewModel taskTableViewModel)
    : BindableBase, INavigationAware
{
    public TaskTableViewModel TaskTableViewModel { get; set; } = taskTableViewModel;

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

                await dataCoordinator.UpdateTaskAsync(task);
            }
        }
    });

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        TaskTableViewModel.Clear();
        _ = ShowAllTasksCommand.ExecuteAsync(null);
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => false;

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }
}
