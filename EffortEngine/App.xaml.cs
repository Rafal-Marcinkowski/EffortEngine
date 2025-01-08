using EffortEngine.MVVM.ViewModels;
using EffortEngine.MVVM.Views;
using System.Windows;

namespace EffortEngine;

public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindowView>();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var regionManager = Container.Resolve<IRegionManager>();
        regionManager.RequestNavigate("MainRegion", nameof(MainMenuView));
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<WorkView>();
        containerRegistry.RegisterForNavigation<ManageTasksView>();
        containerRegistry.RegisterForNavigation<AddProgrammingTaskView>();
        containerRegistry.RegisterForNavigation<AddBugView>();
        containerRegistry.RegisterForNavigation<AddFeatureView>();
        containerRegistry.RegisterForNavigation<AddProgramProjectView>();
        containerRegistry.RegisterForNavigation<MainMenuView>();
        containerRegistry.RegisterForNavigation<AddTaskView>();
        containerRegistry.RegisterForNavigation<SettingsView>();

        containerRegistry.Register<MainMenuViewModel>();
        containerRegistry.Register<AddProgrammingTaskViewModel>();
        containerRegistry.Register<AddBugViewModel>();
        containerRegistry.Register<AddFeatureViewModel>();
        containerRegistry.Register<AddProgramProjectViewModel>();
        containerRegistry.Register<AddTaskViewModel>();
        containerRegistry.Register<WorkViewModel>();
        containerRegistry.Register<ManageTasksView>();
        containerRegistry.RegisterSingleton<SettingsView>();
    }
}
