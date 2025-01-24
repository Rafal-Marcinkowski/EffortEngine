using DataAccess.Data;
using SharedProject.Models;

namespace EffortEngine.MVVM.ViewModels;

public class AddStockMarketTaskViewModel : BindableBase
{
    private string stockMarketTaskName;
    public string StockMarketTaskName
    {
        get => stockMarketTaskName;
        set => SetProperty(ref stockMarketTaskName, value);
    }

    private string stockMarketTaskDescription = string.Empty;
    public string StockMarketTaskDescription
    {
        get => stockMarketTaskDescription;
        set => SetProperty(ref stockMarketTaskDescription, value);
    }

    private string selectedPriority = string.Empty;
    private readonly ITaskData taskData;

    public AddStockMarketTaskViewModel(ITaskData taskData)
    {
        this.taskData = taskData;
    }

    public string SelectedPriority
    {
        get => selectedPriority;
        set => SetProperty(ref selectedPriority, value);
    }


    public IAsyncCommand AddStockMarketTaskCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedPriority is not null)
        {
            StockMarketTask task = new()
            {
                Name = StockMarketTaskName,
                Description = StockMarketTaskDescription,
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
                Type = TaskBase.TaskType.StockMarketTask,
                WorkTime = 0,
            };

            await taskData.InsertTaskAsync(task);

            StockMarketTaskDescription = string.Empty;
            StockMarketTaskName = string.Empty;
            SelectedPriority = string.Empty;
        }
    });
}
