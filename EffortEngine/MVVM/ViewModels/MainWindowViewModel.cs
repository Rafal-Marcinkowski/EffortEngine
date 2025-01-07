using EffortEngine.MVVM.Views;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class MainWindowViewModel(IRegionManager regionManager) : BindableBase
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

    public ICommand WorkCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(WorkView));
    });

    public ICommand MainMenuCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(MainWindowView));
    });

    public ICommand SettingsCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(SettingsView));
    });
}