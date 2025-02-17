using SharedProject.Events;

namespace EffortEngine.MVVM.ViewModels;

public class InformationBarViewModel : BindableBase
{
    private readonly IEventAggregator eventAggregator;
    public WorkManager WorkManager { get; set; }

    public InformationBarViewModel(IEventAggregator eventAggregator, WorkManager workManager)
    {
        this.eventAggregator = eventAggregator;
        this.WorkManager = workManager;
        this.eventAggregator.GetEvent<StartWorkEvent>().Subscribe(async taskName => await WorkManager.OnNewsTaskStarted(taskName));
    }
}




