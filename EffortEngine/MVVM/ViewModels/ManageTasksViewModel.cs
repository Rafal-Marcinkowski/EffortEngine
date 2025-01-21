using DataAccess.Data;
using EffortEngine.MVVM.Views;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class ManageTasksViewModel : BindableBase
{
    private readonly ITaskData taskData;
    private readonly IRegionManager regionManager;
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

    public ManageTasksViewModel(ITaskData taskData, IRegionManager regionManager)
    {
        this.taskData = taskData;
        this.regionManager = regionManager;
        _ = SetTaskTableToAllTasksView();
    }

    private async Task SetTaskTableToAllTasksView()
    {
        var tasks = await taskData.GetAllTasksAsync();

        TaskList = new ObservableCollection<TaskBase>(tasks);
        regionManager.RequestNavigate("TaskTableRegion", nameof(AllTasksTableView));
    }
}
