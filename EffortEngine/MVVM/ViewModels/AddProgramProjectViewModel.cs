using DataAccess.Data;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Views;
using ValidationComponent.ProgramProjects;

namespace EffortEngine.MVVM.ViewModels;

public class AddProgramProjectViewModel(IRegionManager regionManager, IProgramData programData, IEventAggregator eventAggregator) : BindableBase
{

    private string programName = string.Empty;
    public string ProgramName
    {
        get => programName;
        set => SetProperty(ref programName, value);
    }


    public IAsyncCommand AddProgramProjectCommand => new AsyncDelegateCommand(async () =>
    {

        Program program = new()
        {
            Name = ProgramName
        };

        var validator = new AddProgramProjectValidation(programData);
        var results = await validator.ValidateAsync(program);

        if (results.IsValid)
        {
            program.TotalWorkTime = 0;
            await programData.InsertProgramAsync(program);
            ProgramName = string.Empty;
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
    });
}
