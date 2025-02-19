using DataAccess.Data;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class AddBugViewModel : BindableBase
{
    public AddBugViewModel(IProgramData programData, ITaskData taskData)
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

    private readonly IProgramData programData;
    private readonly ITaskData taskData;
    private string bugName = string.Empty;
    public string BugName
    {
        get => bugName;
        set => SetProperty(ref bugName, value);
    }

    private string bugDescription = string.Empty;
    public string BugDescription
    {
        get => bugDescription;
        set => SetProperty(ref bugDescription, value);
    }

    private string bugPriority = string.Empty;
    public string BugPriority
    {
        get => bugPriority;
        set => SetProperty(ref bugPriority, value);
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

    public IAsyncCommand ConfirmProgramSelectionCommand => new AsyncDelegateCommand<Program>(async (selectedProgram) =>
    {
        SelectedProgram = selectedProgram;
        SelectedProgram.Id = await programData.GetProgramIdAsync(SelectedProgram.Name) ?? 0;
    });

    public IAsyncCommand AddBugCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedProgram is not null)
        {
            Bug bug = new()
            {
                Description = BugDescription,
                Name = BugName,
                CreatedAt = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                LastUpdated = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                Status = TaskBase.TaskStatus.NotStarted,
                Type = TaskBase.TaskType.Bug,
                TotalWorkTime = 0,
                Priority = BugPriority switch
                {
                    "Niski" => 0,
                    "Średni" => 1,
                    "Wysoki" => 2,
                    _ => 10
                },

                ProgramId = SelectedProgram.Id,
            };

            await taskData.InsertTaskAsync(bug);

            SelectedProgram = null;
            BugPriority = string.Empty;
            BugName = string.Empty;
            BugDescription = string.Empty;
        }
    });
}
