using DataAccess.Data;

namespace EffortEngine.MVVM.ViewModels;

public class AddGeneralTaskViewModel : BindableBase
{
    private string generalTaskName;
    public string GeneralTaskName
    {
        get => generalTaskName;
        set => SetProperty(ref generalTaskName, value);
    }
    private string generalTaskDescription = string.Empty;
    public string GeneralTaskDescription
    {
        get => generalTaskDescription;
        set => SetProperty(ref generalTaskDescription, value);
    }
    private string selectedPriority = string.Empty;
    private readonly ITaskData taskData;
    public AddGeneralTaskViewModel(ITaskData taskData)
    {
        this.taskData = taskData;
    }
    public string SelectedPriority
    {
        get => selectedPriority;
        set => SetProperty(ref selectedPriority, value);
    }
    public IAsyncCommand AddGeneralTaskCommand => new AsyncDelegateCommand(async () =>
    {

    });
}
