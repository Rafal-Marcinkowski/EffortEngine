using EffortEngine.MVVM.Views;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class MainMenuViewModel(IRegionManager regionManager) : BindableBase
{

    public ICommand AddTaskCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddTaskView));
    });

    public ICommand ManageTasksCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(ManageTasksView));
    });
}
