using DataAccess.Data;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Views;
using ValidationComponent;

namespace EffortEngine.LocalLibrary.Services;

public class TaskManager(ITaskData taskData, IProgramData programData, IEventAggregator eventAggregator) : BindableBase
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

    public async Task<bool> TryAddTask(string taskName, string taskDescription, string taskPriority, int selectedTabIndex, int? programId)
    {
        TaskToAdd = await CreateTask(taskName, taskDescription, taskPriority, selectedTabIndex, programId);
        if (!await ValidateTask(TaskToAdd)) return false;
        await AddTaskAsync(TaskToAdd);
        return true;
    }

    private async Task<TaskBase> CreateTask(string taskName, string taskDescription, string taskPriority, int selectedTabIndex, int? programId)
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

    public async Task AddProgram(string programName)
    {
        Program program = new()
        {
            Name = programName
        };

        var validator = new AddProgramValidation(programData);
        var results = await validator.ValidateAsync(program);

        if (results.IsValid)
        {
            program.TotalWorkTime = 0;
            await programData.InsertProgramAsync(program);
            eventAggregator.GetEvent<ProgramAddedEvent>().Publish(program);
        }

        else
        {
            var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));

            var dialog = new ErrorDialog()
            {
                DialogText = validationErrors
            };

            dialog.ShowDialog();
        }
    }

    public async Task<bool> TryAddProgram(string programName)
    {
        Program program = new()
        {
            Name = programName
        };

        var validator = new AddProgramValidation(programData);
        var results = await validator.ValidateAsync(program);

        if (results.IsValid)
        {
            program.TotalWorkTime = 0;
            await programData.InsertProgramAsync(program);
            eventAggregator.GetEvent<ProgramAddedEvent>().Publish(program);
            return true;
        }

        else
        {
            var validationErrors = string.Join("\n", results.Errors.Select(e => e.ErrorMessage));

            var dialog = new ErrorDialog()
            {
                DialogText = validationErrors
            };

            dialog.ShowDialog();
            return false;
        }
    }
}
