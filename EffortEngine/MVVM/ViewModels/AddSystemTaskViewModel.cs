using DataAccess.Data;
using SharedProject.Models;

namespace EffortEngine.MVVM.ViewModels;

public class AddSystemTaskViewModel : BindableBase
{
    private string systemTaskName;
    public string SystemTaskName
    {
        get => systemTaskName;
        set => SetProperty(ref systemTaskName, value);
    }

    private string systemTaskDescription = string.Empty;
    public string SystemTaskDescription
    {
        get => systemTaskDescription;
        set => SetProperty(ref systemTaskDescription, value);
    }

    private string selectedPriority = string.Empty;
    private readonly ITaskData taskData;

    public AddSystemTaskViewModel(ITaskData taskData)
    {
        this.taskData = taskData;
    }

    public string SelectedPriority
    {
        get => selectedPriority;
        set => SetProperty(ref selectedPriority, value);
    }

    public IAsyncCommand AddSystemTaskCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedPriority is not null)
        {
            SystemTask task = new()
            {
                Name = SystemTaskName,
                Description = SystemTaskDescription,
                Priority = SelectedPriority switch
                {
                    "Niski" => 0,
                    "Średni" => 1,
                    "Wysoki" => 2,
                    _ => 10
                },
                CreatedAt = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                LastUpdated = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                Status = TaskBase.TaskStatus.NotStarted,
                Type = TaskBase.TaskType.SystemTask,
                TotalWorkTime = 0,
            };

            await taskData.InsertTaskAsync(task);

            SystemTaskDescription = string.Empty;
            SystemTaskName = string.Empty;
            SelectedPriority = string.Empty;
        }
    });

}
