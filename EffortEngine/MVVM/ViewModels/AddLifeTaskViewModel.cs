using DataAccess.Data;
using SharedProject.Models;

namespace EffortEngine.MVVM.ViewModels;

public class AddLifeTaskViewModel : BindableBase
{
    private string lifeTaskName;
    public string LifeTaskName
    {
        get => lifeTaskName;
        set => SetProperty(ref lifeTaskName, value);
    }

    private string lifeTaskDescription = string.Empty;
    public string LifeTaskDescription
    {
        get => lifeTaskDescription;
        set => SetProperty(ref lifeTaskDescription, value);
    }

    private string selectedPriority = string.Empty;
    private readonly ITaskData taskData;

    public AddLifeTaskViewModel(ITaskData taskData)
    {
        this.taskData = taskData;
    }

    public string SelectedPriority
    {
        get => selectedPriority;
        set => SetProperty(ref selectedPriority, value);
    }


    public IAsyncCommand AddLifeTaskCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedPriority is not null)
        {
            LifeTask task = new()
            {
                Name = LifeTaskName,
                Description = LifeTaskDescription,
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
                Type = TaskBase.TaskType.LifeTask,
                TotalWorkTime = 0,
            };

            await taskData.InsertTaskAsync(task);

            LifeTaskDescription = string.Empty;
            LifeTaskName = string.Empty;
            SelectedPriority = string.Empty;
        }
    });
}
