using EffortEngine.LocalLibrary;
using EffortEngine.MVVM.Views;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class TaskTableViewModel(IRegionManager regionManager, ViewManager viewManager) : BindableBase
{
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

    private ObservableCollection<Program> programList;
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

    public string SelectedTaskName => SelectedProgram?.Name ?? SelectedTask?.Name ?? string.Empty;
    public string SelectedTaskStatus => SelectedTask?.Status.ToString() ?? string.Empty;

    public async Task RefreshUI(int id)
    {
        var task = await viewManager.GetTaskByIdAsync(id);
        if (task == null)
            return;

        if (task.ProgramId is not null)
        {
            var program = await viewManager.GetProgramByIdAsync(task.ProgramId.Value);
            if (program != null)
                program.Name = program.Name;
        }

        var existingTask = TaskList.FirstOrDefault(t => t.Id == task.Id);
        if (existingTask != null)
        {
            int index = TaskList.IndexOf(existingTask);
            TaskList[index] = task;
        }
    }

    public async Task ShowPrograms()
    {
        ProgramList = [.. await viewManager.GetProgramsWithTasksAsync()];
        regionManager.Regions["TaskTableRegion"].RemoveAll();
        regionManager.RequestNavigate("TaskTableRegion", nameof(ProgramsTaskTableView));
    }

    public async Task ShowAllTasks()
    {
        TaskList = [.. await viewManager.GetAllTasksAsync()];
        regionManager.Regions["TaskTableRegion"].RemoveAll();
        regionManager.RequestNavigate("TaskTableRegion", nameof(AllTasksTableView));
    }

    public async Task ShowAllProgrammingTasks()
    {
        TaskList = [.. await viewManager.GetProgrammingTasksAsync()];
        regionManager.Regions["TaskTableRegion"].RemoveAll();
        regionManager.RequestNavigate("TaskTableRegion", nameof(TaskTableView));
    }

    public async Task ShowSystemTasks()
    {
        TaskList = [.. await viewManager.GetTasksByTypeAsync(TaskBase.TaskType.SystemTask)];
        regionManager.Regions["TaskTableRegion"].RemoveAll();
        regionManager.RequestNavigate("TaskTableRegion", nameof(TaskTableView));
    }
    public async Task ShowStockMarketTasks()
    {
        TaskList = [.. await viewManager.GetTasksByTypeAsync(TaskBase.TaskType.StockMarketTask)];
        regionManager.Regions["TaskTableRegion"].RemoveAll();
        regionManager.RequestNavigate("TaskTableRegion", nameof(TaskTableView));
    }

    public async Task ShowLifeTasks()
    {
        TaskList = [.. await viewManager.GetTasksByTypeAsync(TaskBase.TaskType.LifeTask)];
        regionManager.Regions["TaskTableRegion"].RemoveAll();
        regionManager.RequestNavigate("TaskTableRegion", nameof(TaskTableView));
    }

    internal void Clear()
    {
        SelectedProgram = null;
        SelectedTask = null;
    }
}
