using EffortEngine.LocalLibrary.Services;

namespace EffortEngine.MVVM.ViewModels;

public class PomodoroBarViewModel(WorkManager workManager) : BindableBase
{
    public WorkManager WorkManager { get; set; } = workManager;
}