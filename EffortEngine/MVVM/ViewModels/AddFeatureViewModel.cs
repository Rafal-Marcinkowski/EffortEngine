using DataAccess.Data;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class AddFeatureViewModel : BindableBase
{
    private readonly IProgramData programData;
    private readonly ITaskData taskData;

    public AddFeatureViewModel(IProgramData programData, ITaskData taskData)
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

    private string featureName = string.Empty;
    public string FeatureName
    {
        get => featureName;
        set => SetProperty(ref featureName, value);
    }

    private string featureDescription = string.Empty;
    public string FeatureDescription
    {
        get => featureDescription;
        set => SetProperty(ref featureDescription, value);
    }

    private string featurePriority = string.Empty;
    public string FeaturePriority
    {
        get => featurePriority;
        set => SetProperty(ref featurePriority, value);
    }

    public IAsyncCommand ConfirmProgramSelectionCommand => new AsyncDelegateCommand<Program>(async (selectedProgram) =>
    {
        SelectedProgram = selectedProgram;
        SelectedProgram.Id = await programData.GetProgramIdAsync(SelectedProgram.Name) ?? 0;
    });

    public IAsyncCommand AddFeatureCommand => new AsyncDelegateCommand(async () =>
    {
        if (SelectedProgram is not null)
        {
            Feature feature = new()
            {
                Description = FeatureDescription,
                Name = FeatureName,
                CreatedAt = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                LastUpdated = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute),
                Status = TaskBase.TaskStatus.NotStarted,
                Type = TaskBase.TaskType.Feature,
                WorkTime = 0,
                Priority = FeaturePriority switch
                {
                    "Niski" => 0,
                    "Średni" => 1,
                    "Wysoki" => 2,
                    _ => 10
                },
                ProgramId = SelectedProgram.Id,
            };

            await taskData.InsertTaskAsync(feature);

            SelectedProgram = null;
            FeatureDescription = string.Empty;
            FeaturePriority = null;
            FeatureName = string.Empty;
        }
    });
}
