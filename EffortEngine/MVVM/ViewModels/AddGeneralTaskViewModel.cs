using DataAccess.Data;
using EffortEngine.LocalLibrary;
using MahApps.Metro.Controls.Dialogs;

namespace EffortEngine.MVVM.ViewModels;

public class AddGeneralTaskViewModel(ITaskData taskData, TaskManager taskManager) : BindableBase
{
    private string taskName = string.Empty;
    public string TaskName
    {
        get => taskName;
        set => SetProperty(ref taskName, value);
    }

    private string taskDescription = string.Empty;
    public string TaskDescription
    {
        get => taskDescription;
        set => SetProperty(ref taskDescription, value);
    }

    private string taskPriority = string.Empty;
    public string TaskPriority
    {
        get => taskPriority;
        set => SetProperty(ref taskPriority, value);
    }

    private int selectedTabIndex;
    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set
        {
            if (SetProperty(ref selectedTabIndex, value))
            {
                HandleTabSelectionChanged();
            }
        }
    }

    public IAsyncCommand AddGeneralTaskCommand => new AsyncDelegateCommand(async () =>
    {
        if (!await taskManager.TryAddTask(TaskName, TaskDescription, TaskPriority, SelectedTabIndex)) return;
        await dialogCoordinator.ShowMessageAsync(this, "Dodano zadanie",
           $"Nazwa: {TaskName}\n{TaskManager.TaskToAdd.Type} został dodany.");
        await ClearFields();
    });

    public async Task HandleTabSelectionChanged()
    {
        await Task.Delay(50);
        await ClearFields();
    }

    private async Task ClearFields()
    {
        TaskPriority = null;
        TaskName = null;
        TaskDescription = null;
    }
}
