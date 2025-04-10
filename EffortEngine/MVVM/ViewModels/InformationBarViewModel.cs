using EffortEngine.LocalLibrary;

namespace EffortEngine.MVVM.ViewModels;

public class InformationBarViewModel(WorkManager workManager) : BindableBase
{
    public WorkManager WorkManager { get; set; } = workManager;
}