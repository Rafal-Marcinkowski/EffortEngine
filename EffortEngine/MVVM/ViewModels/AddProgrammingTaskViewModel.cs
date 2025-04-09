using DataAccess.Data;
using EffortEngine.LocalLibrary;
using MahApps.Metro.Controls.Dialogs;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Views;
using System.Collections.ObjectModel;
using ValidationComponent;

namespace EffortEngine.MVVM.ViewModels;

public class AddProgrammingTaskViewModel : BindableBase
{
    private readonly IProgramData programData;
    private readonly IEventAggregator eventAggregator;
    private readonly IDialogCoordinator dialogCoordinator;
    private readonly TaskManager taskManager;

    public AddProgrammingTaskViewModel(IProgramData programData,
        IEventAggregator eventAggregator, IDialogCoordinator dialogCoordinator, TaskManager taskManager)
    {
        this.programData = programData;
        this.eventAggregator = eventAggregator;
        this.dialogCoordinator = dialogCoordinator;
        this.taskManager = taskManager;
        LoadProgramIdCommand.Execute(null);
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

    private Program? selectedProgram;
    public Program? SelectedProgram
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

    public IAsyncCommand ResetCommand => new AsyncDelegateCommand(async () => await ClearFields());

    public IAsyncCommand AddTaskCommand => new AsyncDelegateCommand(async () =>
    {
        if (!await taskManager.TryAddTask(TaskName, TaskDescription, TaskPriority, SelectedTabIndex, SelectedProgram?.Id ?? 0)) return;

        await dialogCoordinator.ShowMessageAsync(this, "Dodano zadanie",
           $"Nazwa: {TaskName}\n{TaskManager.TaskToAdd.Type} został dodany do programu {SelectedProgram?.Name}.");
        await ClearFields();
    });

    public IAsyncCommand AddProgramCommand => new AsyncDelegateCommand(async () =>
    {
        Program program = new()
        {
            Name = TaskName
        };

        var validator = new AddProgramValidation(programData);
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

    private async Task ClearFields()
    {
        TaskPriority = null;
        TaskName = null;
        TaskDescription = null;
        SelectedProgram = null;
    }
}
