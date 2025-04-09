using DataAccess.Data;
using SharedProject.Models;
using SharedProject.Views;
using ValidationComponent;

namespace EffortEngine.LocalLibrary;

public class TaskManager(ITaskData taskData, IProgramData programData) : BindableBase
{
    public static TaskBase CurrentTask { get; set; }
    public static Program CurrentProgram { get; set; }

    public static TaskBase TaskToAdd { get; set; }

    public async Task EvaluateTask(string taskName)
    {
        var task = (await taskData.GetAllTasksAsync()).FirstOrDefault(q => q.Name == taskName);

        if (task is not null)
        {
            CurrentTask = task;
            CurrentProgram = null;
        }

        else
        {
            CurrentProgram = (await programData.GetAllProgramsAsync()).FirstOrDefault(q => q.Name == taskName);
            CurrentTask = null;
        }
    }

    public async Task AddTaskAsync(TaskBase task)
    {

        await taskData.InsertTaskAsync(task);
    }

    public static async Task<bool> ValidateTask(TaskBase task)
    {
        var validator = new AddTaskValidation();

        var validationResult = await validator.ValidateAsync(task);

        if (!validationResult.IsValid)
        {
            var validationErrors = string.Join("\n", validationResult.Errors.Select(e => e.ErrorMessage));
            var dialog = new ErrorDialog()
            {
                DialogText = validationErrors
            };
            dialog.ShowDialog();
            return false;
        }

        return true;
    }

    public async Task<bool> TryAddTask(string taskName, string taskDescription, string taskPriority, int selectedTabIndex, int programId)
    {
        TaskToAdd = await CreateTask(taskName, taskDescription, taskPriority, selectedTabIndex, programId);
        if (!await ValidateTask(TaskToAdd)) return false;
        await AddTaskAsync(TaskToAdd);
        return true;
    }

    private async Task<TaskBase> CreateTask(string taskName, string taskDescription, string taskPriority, int selectedTabIndex, int programId)
    {
        return new TaskBase
        {
            Name = taskName,
            Description = taskDescription,
            CreatedAt = DateTime.Now,
            LastUpdated = DateTime.Now,
            Status = TaskBase.TaskStatus.NotStarted,
            Type = (TaskBase.TaskType)selectedTabIndex,
            TotalWorkTime = 0,
            Priority = taskPriority switch
            {
                "Niski" => 0,
                "Średni" => 1,
                "Wysoki" => 2,
                _ => 10
            },
            ProgramId = programId
        };
    }
}
