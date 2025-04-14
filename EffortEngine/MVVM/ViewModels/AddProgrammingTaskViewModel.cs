using DataAccess.Data;
using EffortEngine.LocalLibrary;
using MahApps.Metro.Controls.Dialogs;
using SharedProject.Models;
using System.Collections.ObjectModel;

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
        if (!await taskManager.TryAddTask(TaskName, TaskDescription, TaskPriority, SelectedTabIndex, SelectedProgram?.Id ?? null)) return;

        await dialogCoordinator.ShowMessageAsync(this, "Dodano zadanie",
           $"Nazwa: {TaskName}\n{TaskManager.TaskToAdd.Type} został dodany do programu {SelectedProgram?.Name}.");
        await ClearFields();
    });

    public IAsyncCommand AddProgramCommand => new AsyncDelegateCommand(async () =>
    {
        if (await taskManager.TryAddProgram(TaskName))
        {
            await dialogCoordinator.ShowMessageAsync(this, "Dodano program", $"Program {TaskName} został dodany.");
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
