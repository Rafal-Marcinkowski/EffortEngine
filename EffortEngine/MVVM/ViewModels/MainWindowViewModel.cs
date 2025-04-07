using EffortEngine.MVVM.Views;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class MainWindowViewModel(IRegionManager regionManager, IDialogCoordinator dialogCoordinator) : BindableBase
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

    public ICommand MainMenuCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(MainMenuView));
    });

    public ICommand SettingsCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(SettingsView));
    });
}