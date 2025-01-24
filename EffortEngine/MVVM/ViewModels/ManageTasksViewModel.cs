using DataAccess.Data;
using EffortEngine.MVVM.Views;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class ManageTasksViewModel : BindableBase
{
    private readonly ITaskData taskData;
    private readonly IRegionManager regionManager;
    private readonly IProgramData programData;
    private readonly WorkManager workManager;

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

    public ManageTasksViewModel(ITaskData taskData, IRegionManager regionManager, IProgramData programData, WorkManager workManager)
    {
        this.taskData = taskData;
        this.regionManager = regionManager;
        this.programData = programData;
        this.workManager = workManager;
        ShowAllTasksCommand.ExecuteAsync(this);
    }

    private async Task SetTaskTableToAllTasksView()
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = new ObservableCollection<TaskBase>(tasks);

        regionManager.RequestNavigate("TaskTableRegion", nameof(AllTasksTableView));
    }

    public IAsyncCommand ShowProgramsCommand => new AsyncDelegateCommand(async () =>
    {
        var programs = await programData.GetAllProgramsAsync();
        ProgramList = new ObservableCollection<Program>(programs);
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
        TaskList = new ObservableCollection<TaskBase>(tasks);

        regionManager.RequestNavigate("TaskTableRegion", nameof(AllTasksTableView));
    });

    public IAsyncCommand ShowAllProgrammingTasksCommand => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = new ObservableCollection<TaskBase>(tasks.Where(q => q.ProgramId != null));

        regionManager.RequestNavigate("TaskTableRegion", nameof(AllProgrammingTasksView));
    });

    public IAsyncCommand ShowSystemTasksCommand => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = new ObservableCollection<TaskBase>(tasks.Where(q => q.Type == TaskBase.TaskType.SystemTask));

        regionManager.RequestNavigate("TaskTableRegion", nameof(SystemTasksView));
    });
    public IAsyncCommand ShowStockMarketTasksView => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = new ObservableCollection<TaskBase>(tasks.Where(q => q.Type == TaskBase.TaskType.StockMarketTask));

        regionManager.RequestNavigate("TaskTableRegion", nameof(StockMarketTasksView));
    });

    public IAsyncCommand ShowLifeTasksView => new AsyncDelegateCommand(async () =>
    {
        var tasks = await taskData.GetAllTasksAsync();
        TaskList = new ObservableCollection<TaskBase>(tasks.Where(q => q.Type == TaskBase.TaskType.LifeTask));

        regionManager.RequestNavigate("TaskTableRegion", nameof(LifeTasksView));
    });


    public IAsyncCommand StartWorkCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedTask is not null || SelectedProgram is not null)
        {
            if (!workManager.IsSessionAlive)
            {

            }
        }



    });
}
