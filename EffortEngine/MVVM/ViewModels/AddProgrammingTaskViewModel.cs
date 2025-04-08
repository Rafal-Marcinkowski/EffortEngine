using DataAccess.Data;
using MahApps.Metro.Controls.Dialogs;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Views;
using System.Collections.ObjectModel;
using ValidationComponent.ProgramProjects;

namespace EffortEngine.MVVM.ViewModels;

public class AddProgrammingTaskViewModel : BindableBase
{
    private readonly IProgramData programData;
    private readonly ITaskData taskData;
    private readonly IEventAggregator eventAggregator;
    private readonly IDialogCoordinator dialogCoordinator;

    public AddProgrammingTaskViewModel(IProgramData programData, ITaskData taskData,
        IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator)
    {
        this.programData = programData;
        this.taskData = taskData;
        this.eventAggregator = eventAggregator;
        this.dialogCoordinator = dialogCoordinator;
        _ = GetProgramsAsync();
    }

    private async Task GetProgramsAsync()
    {
        var results = await programData.GetAllProgramsAsync();

        Programs = [.. results];
    }

    private ObservableCollection<Program> programs = [];
    public ObservableCollection<Program> Programs
    {
        get => programs;
        set => SetProperty(ref programs, value);
    }

    private Program selectedProgram;
    public Program SelectedProgram
    {
        get => selectedProgram;
        set
        {
            if (SetProperty(ref selectedProgram, value))
            {
                LoadProgramIdCommand.Execute(null);
            }
        }
    }

    private int selectedTabIndex;
    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set
        {
            if (SetProperty(ref selectedTabIndex, value))
            {
                TabSelectionChangedCommand.Execute(null);
            }
        }
    }

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
    public IAsyncCommand LoadProgramIdCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedProgram != null)
        {
            var id = await programData.GetProgramIdAsync(SelectedProgram.Name);
            SelectedProgram.Id = id ?? 0;
        }
    });

    public IAsyncCommand ResetCommand => new AsyncDelegateCommand(async () =>
    {
        await ClearFields();
    });

    public IAsyncCommand AddTaskCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedProgram is null) return;

        await (SelectedTabIndex switch
        {
            0 => AddTaskAsync<Bug>(TaskBase.TaskType.Bug),
            1 => AddTaskAsync<Feature>(TaskBase.TaskType.Feature),
            2 => AddTaskAsync<Module>(TaskBase.TaskType.Module),
            _ => ClearFields()
        });

        await ClearFields();
    });

    private async Task ClearFields()
    {
        TaskPriority = null;
        TaskName = null;
        TaskDescription = null;
        SelectedProgram = null;
    }

    private async Task AddTaskAsync<T>(TaskBase.TaskType taskType) where T : TaskBase, new()
    {
        var task = new T
        {
            Description = TaskDescription,
            Name = TaskName,
            CreatedAt = DateTime.Now,
            LastUpdated = DateTime.Now,
            Status = TaskBase.TaskStatus.NotStarted,
            Type = taskType,
            TotalWorkTime = 0,
            Priority = TaskPriority switch
            {
                "Niski" => 0,
                "Średni" => 1,
                "Wysoki" => 2,
                _ => 10
            },
            ProgramId = SelectedProgram.Id,
        };

        await dialogCoordinator.ShowMessageAsync(this, $"Dodano {taskType}", $"{TaskName} został dodany do programu {SelectedProgram.Name}.");
        await taskData.InsertTaskAsync(task);
    }

    public IAsyncCommand AddProgramCommand => new AsyncDelegateCommand(async () =>
    {

        Program program = new()
        {
            Name = TaskName
        };

        var validator = new AddProgramProjectValidation(programData);
        var results = await validator.ValidateAsync(program);

        if (results.IsValid)
        {
            program.TotalWorkTime = 0;
            await programData.InsertProgramAsync(program);
            eventAggregator.GetEvent<ProgramAddedEvent>().Publish(program);
            await dialogCoordinator.ShowMessageAsync(this, "Dodano program", $"Program {program.Name} został dodany.");
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

        await ClearFields();
    });

    public IAsyncCommand TabSelectionChangedCommand => new AsyncDelegateCommand(async () =>
    {
        await Task.Delay(50);
        await ClearFields();
    });
}
