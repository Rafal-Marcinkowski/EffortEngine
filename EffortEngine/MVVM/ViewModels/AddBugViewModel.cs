using DataAccess.Data;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace EffortEngine.MVVM.ViewModels;

public class AddBugViewModel : BindableBase
{
    private ObservableCollection<Program> programs = [];
    public ObservableCollection<Program> Programs
    {
        get => programs;
        set => SetProperty(ref programs, value);
    }

    private readonly IProgramData programData;

    public AddBugViewModel(IProgramData programData)
    {
        this.programData = programData;
        GetProgramsAsync();
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
    });
}
