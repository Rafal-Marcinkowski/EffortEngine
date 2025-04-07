using DataAccess.Data;
using EffortEngine.LocalLibrary;
using EffortEngine.MVVM.Views;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Views;
using System.Collections.ObjectModel;
using System.Windows;

namespace EffortEngine.MVVM.ViewModels;

public class ManageTasksViewModel : BindableBase, INavigationAware
{
    private readonly ITaskData taskData;
    private readonly IRegionManager regionManager;
    private readonly IProgramData programData;
    private readonly IEventAggregator eventAggregator;

    private string SelectedTaskName => SelectedProgram?.Name ?? SelectedTask?.Name ?? string.Empty;
    private string SelectedTaskStatus => SelectedTask?.Status.ToString() ?? string.Empty;

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

    public ManageTasksViewModel(ITaskData taskData, IRegionManager regionManager, IProgramData programData
        , IEventAggregator eventAggregator)
    {
        this.taskData = taskData;
        this.regionManager = regionManager;
        this.programData = programData;

        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<WorkTimeAddedEvent>().Subscribe(async (id) => await RefreshUI(id));

        ShowAllTasksCommand.ExecuteAsync(this);
    }

    private async Task RefreshUI(int id)
    {
        //MessageBox.Show(id.ToString());

        //var task = TaskList.FirstOrDefault(t => t.Id == id);
        //if (task != null)
        //{
        //    var updatedTask = await taskData.GetTaskAsync(id);
        //    if (updatedTask != null)
        //    {
        //        TaskList.Remove(task);
        //        TaskList.Add(updatedTask);
        //        RaisePropertyChanged(nameof(TaskList));
        //    }
        //    return;
        //}

        //var program = ProgramList.FirstOrDefault(p => p.Id == id);
        //if (program != null)
        //{
        //    var updatedProgram = await programData.GetProgramAsync(id);
        //    if (updatedProgram != null)
        //    {
        //        ProgramList.Remove(program);
        //        ProgramList.Add(updatedProgram);
        //        RaisePropertyChanged(nameof(ProgramList));
        //    }
        //}
    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        SelectedProgram = null;
        SelectedTask = null;
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return false;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {

    }

    public IAsyncCommand ShowProgramsCommand => new AsyncDelegateCommand(async () =>
    {
        var programs = await programData.GetAllProgramsAsync();
        ProgramList = [.. programs];
        var allTasks = await taskData.GetAllTasksAsync();

        foreach (var program in programs)
        {
            var programId = await programData.GetProgramIdAsync(program.Name);

            var bugsForProgram = allTasks
                .Where(q => q.ProgramId == programId && q.Type == TaskBase.TaskType.Bug)
                .OfType<Bug>()
                .ToList();
            program.Bugs = bugsForProgram;

            var featuresForProgram = allTasks
                .Where(q => q.ProgramId == programId && q.Type == TaskBase.TaskType.Feature)
                .OfType<Feature>()
                .ToList();
            program.Features = featuresForProgram;

            var modulesForProgram = allTasks
                .Where(q => q.ProgramId == programId && q.Type == TaskBase.TaskType.Module)
                .OfType<Module>()
                .ToList();
            program.Modules = modulesForProgram;
        }

        regionManager.RequestNavigate("TaskTableRegion", nameof(ProgramsTaskTableView));
    });

    public IAsyncCommand ShowAllTasksCommand => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = [.. tasks];

        regionManager.RequestNavigate("TaskTableRegion", nameof(AllTasksTableView));
    });

    public IAsyncCommand ShowAllProgrammingTasksCommand => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = [.. tasks.Where(q => q.ProgramId != null)];

        regionManager.RequestNavigate("TaskTableRegion", nameof(AllProgrammingTasksView));
    });

    public IAsyncCommand ShowSystemTasksCommand => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = [.. tasks.Where(q => q.Type == TaskBase.TaskType.SystemTask)];

        regionManager.RequestNavigate("TaskTableRegion", nameof(SystemTasksView));
    });

    public IAsyncCommand ShowStockMarketTasksView => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = [.. tasks.Where(q => q.Type == TaskBase.TaskType.StockMarketTask)];

        regionManager.RequestNavigate("TaskTableRegion", nameof(StockMarketTasksView));
    });

    public IAsyncCommand ShowLifeTasksView => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = [.. tasks.Where(q => q.Type == TaskBase.TaskType.LifeTask)];

        regionManager.RequestNavigate("TaskTableRegion", nameof(LifeTasksView));
    });

    public IAsyncCommand StartWorkCommand => new AsyncDelegateCommand(async () =>
    {
        if (!String.IsNullOrEmpty(SelectedTaskName) && SelectedTaskStatus != nameof(TaskBase.TaskStatus.Completed))
        {
            var dialog = new ConfirmationDialog { DialogText = $"Rozpocząć pracę nad: {SelectedTaskName}?" };
            dialog.ShowDialog();

            if (dialog.Result)
            {
                eventAggregator.GetEvent<StartWorkEvent>().Publish(SelectedTaskName);
            }
        }
    });

    public IAsyncCommand CompleteTaskCommand => new AsyncDelegateCommand<TaskBase>(async task =>
    {
        if (task != null)
        {
            var dialog = new ConfirmationDialog { DialogText = $"Zakończyć zadanie: {SelectedTaskName}?" };
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

                await taskData.UpdateTaskAsync(task);
            }

            else
            {
                MessageBox.Show($"{TaskManager.CurrentTask?.Id} {PomodoroTimer.ActiveWorkMinutes}");
            }
        }
    });
}
