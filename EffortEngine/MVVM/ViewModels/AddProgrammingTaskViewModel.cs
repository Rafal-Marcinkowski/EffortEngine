using EffortEngine.MVVM.Views;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class AddProgrammingTaskViewModel(IRegionManager regionManager) : BindableBase
{

    public ICommand AddProgramProjectCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddProgramProjectView));
    });

    public ICommand AddModuleCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddModuleView));
    });

    public ICommand AddFeatureCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddFeatureView));
    });

    public ICommand AddBugCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddBugView));
    });
}
