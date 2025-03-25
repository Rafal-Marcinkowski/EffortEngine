using DataAccess.Data;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class AddModuleViewModel : BindableBase
{
    private readonly IProgramData programData;
    private readonly ITaskData taskData;

    public AddModuleViewModel(IProgramData programData, ITaskData taskData)
    {
        this.programData = programData;
        this.taskData = taskData;
        GetProgramsAsync();
    }

    private ObservableCollection<Program> programs = [];
    public ObservableCollection<Program> Programs
    {
        get => programs;
        set => SetProperty(ref programs, value);
    }

    private async Task GetProgramsAsync()
    {
        var results = await programData.GetAllProgramsAsync();

        Programs = new ObservableCollection<Program>(results);
    }

    private Program selectedProgram;
    public Program SelectedProgram
    {
        get => selectedProgram;
        set => SetProperty(ref selectedProgram, value);
    }

    private string moduleName = string.Empty;
    public string ModuleName
    {
        get => moduleName;
        set => SetProperty(ref moduleName, value);
    }

    private string moduleDescription = string.Empty;
    public string ModuleDescription
    {
        get => moduleDescription;
        set => SetProperty(ref moduleDescription, value);
    }

    private string modulePriority = string.Empty;
    public string ModulePriority
    {
        get => modulePriority;
        set => SetProperty(ref modulePriority, value);
    }

    public IAsyncCommand ConfirmProgramSelectionCommand => new AsyncDelegateCommand<Program>(async (selectedProgram) =>
    {
        SelectedProgram = selectedProgram;
        SelectedProgram.Id = await programData.GetProgramIdAsync(SelectedProgram.Name) ?? 0;
    });

    public IAsyncCommand AddModuleCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedProgram is not null)
        {
            Module module = new()
            {
                Description = ModuleDescription,
                Name = ModuleName,
                CreatedAt = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                LastUpdated = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                Status = TaskBase.TaskStatus.NotStarted,
                Type = TaskBase.TaskType.Module,
                TotalWorkTime = 0,
                Priority = ModulePriority switch
                {
                    "Niski" => 0,
                    "Średni" => 1,
                    "Wysoki" => 2,
                    _ => 10
                },
                ProgramId = SelectedProgram.Id,
            };

            await taskData.InsertTaskAsync(module);

            SelectedProgram = null;
            ModuleDescription = string.Empty;
            ModulePriority = null;
            ModuleName = string.Empty;
        }
    });
}
