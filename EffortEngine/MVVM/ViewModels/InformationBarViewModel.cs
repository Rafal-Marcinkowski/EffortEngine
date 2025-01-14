using SharedProject.Events;
using SharedProject.Models;

namespace EffortEngine.MVVM.ViewModels;

public class InformationBarViewModel : BindableBase
{
    private readonly IEventAggregator eventAggregator;

    public InformationBarViewModel(IEventAggregator eventAggregator)
    {
        this.eventAggregator = eventAggregator;

        this.eventAggregator.GetEvent<ProgramAddedEvent>().Subscribe(async (program) =>
        {
            await OnProgramAdded(program);
        });
    }

    private async Task OnProgramAdded(Program program)
    {
        RecentActionText = $"Dodano nowy program: {program.Name}";
    }

    private string recentActionText = string.Empty;
    public string RecentActionText
    {
        get => recentActionText;
        set => SetProperty(ref recentActionText, value);
    }
}
