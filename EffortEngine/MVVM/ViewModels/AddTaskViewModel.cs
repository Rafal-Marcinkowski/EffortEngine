using EffortEngine.MVVM.Views;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class AddTaskViewModel(IRegionManager regionManager) : BindableBase
{

    public ICommand AddProgrammingTaskCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddProgrammingTaskView));
    });

    public ICommand AddStockMarketTaskCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddStockMarketTaskView));
    });

    public ICommand AddLifeTaskCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddLifeTaskView));
    });

    public ICommand AddSystemTaskCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddSystemTaskView));
    });
}
