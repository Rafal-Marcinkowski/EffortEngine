using DataAccess.Data;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class AddProgrammingTaskViewModel : BindableBase
{
    private readonly IRegionManager regionManager;
    private readonly IProgramData programData;
    private readonly ITaskData taskData;

    public AddProgrammingTaskViewModel(IRegionManager regionManager, IProgramData programData, ITaskData taskData)
    {
        this.regionManager = regionManager;
        this.programData = programData;
        this.taskData = taskData;
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
        set => SetProperty(ref selectedProgram, value);
    }

    public IAsyncCommand ConfirmProgramSelectionCommand => new AsyncDelegateCommand<Program>(async (selectedProgram) =>
    {
        SelectedProgram = selectedProgram;
        SelectedProgram.Id = await programData.GetProgramIdAsync(SelectedProgram.Name) ?? 0;
    });

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

    public IAsyncCommand AddTaskCommand => new AsyncDelegateCommand(async () =>
    {

    });

    public IAsyncCommand AddProgramCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedProgram is not null)
        {

        }
    });
}
