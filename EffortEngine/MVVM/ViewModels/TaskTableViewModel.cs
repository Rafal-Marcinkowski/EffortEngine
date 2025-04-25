using EffortEngine.LocalLibrary.Services;
using EffortEngine.MVVM.Views;
using SharedProject.Events;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class TaskTableViewModel : BindableBase, INavigationAware
{
    public TaskTableViewModel(ViewManager viewManager, DataCoordinator dataCoordinator, IEventAggregator eventAggregator)
    {
        this.viewManager = viewManager;
        this.dataCoordinator = dataCoordinator;
        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<WorkTimeAddedEvent>().Subscribe(async (eventArgs) => await RefreshUI(eventArgs));
    }

    private readonly ViewManager viewManager;
    private readonly DataCoordinator dataCoordinator;
    private readonly IEventAggregator eventAggregator;

    public string SelectedTaskName => SelectedProgram?.Name ?? SelectedTask?.Name ?? string.Empty;
    public string SelectedTaskStatus => SelectedTask?.Status.ToString() ?? string.Empty;

    private Program selectedProgram;
    public Program SelectedProgram
    {
        get => selectedProgram;
        set => SetProperty(ref selectedProgram, value);
    }

    private TaskBase selectedTask;
    public TaskBase SelectedTask
    {
        get => selectedTask;
        set => SetProperty(ref selectedTask, value);
    }

    private ObservableCollection<Program> programList = [];
    public ObservableCollection<Program> ProgramList
    {
        get => programList;
        set => SetProperty(ref programList, value);
    }

    private ObservableCollection<TaskBase> taskList = [];
    public ObservableCollection<TaskBase> TaskList
    {
        get => taskList;
        set => SetProperty(ref taskList, value);
    }

    public async Task RefreshUI(WorkTimeAddedEventArgs eventArgs)
    {
        if (eventArgs.IsProgram)
        {
            var program = ProgramList.FirstOrDefault(q => q.Id == eventArgs.Id);
            if (program != null)
            {
                RaisePropertyChanged(nameof(ProgramList));
                program.TotalWorkTime += eventArgs.WorkTimeToAdd;
            }
        }
        else
        {
            var task = TaskList.FirstOrDefault(q => q.Id == eventArgs.Id);
            if (task != null)
            {
                RaisePropertyChanged(nameof(TaskList));
                task.TotalWorkTime += eventArgs.WorkTimeToAdd;
            }
        }
    }

    public async Task ShowPrograms()
    {
        Clear();
        ProgramList = [.. await dataCoordinator.GetProgramsWithTasksAsync()];
        await viewManager.NavigateToView("TaskTableRegion", nameof(ProgramsTaskTableView));
    }

    public async Task ShowAllTasks()
    {
        TaskList = [.. await dataCoordinator.GetAllTasksAsync()];
        await viewManager.NavigateToView("TaskTableRegion", nameof(AllTasksTableView));
    }

    public async Task ShowAllProgrammingTasks()
    {
        TaskList = [.. await dataCoordinator.GetProgrammingTasksAsync()];
        await viewManager.NavigateToView("TaskTableRegion", nameof(TaskTableView));
    }

    public async Task ShowSystemTasks()
    {
        TaskList = [.. await dataCoordinator.GetTasksByTypeAsync(TaskBase.TaskType.SystemTask)];
        await viewManager.NavigateToView("TaskTableRegion", nameof(TaskTableView));
    }

    public async Task ShowStockMarketTasks()
    {
        TaskList = [.. await dataCoordinator.GetTasksByTypeAsync(TaskBase.TaskType.StockMarketTask)];
        await viewManager.NavigateToView("TaskTableRegion", nameof(TaskTableView));
    }

    public async Task ShowLifeTasks()
    {
        TaskList = [.. await dataCoordinator.GetTasksByTypeAsync(TaskBase.TaskType.LifeTask)];
        await viewManager.NavigateToView("TaskTableRegion", nameof(TaskTableView));
    }

    internal void Clear()
    {
        SelectedProgram = null;
        SelectedTask = null;
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        Clear();
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return false;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
        return;
    }
}
