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

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterForNavigation<WorkView>();
        containerRegistry.RegisterForNavigation<ManageTasksView>();
        containerRegistry.RegisterForNavigation<AddTaskView>();
        containerRegistry.RegisterForNavigation<SettingsView>();

        containerRegistry.Register<AddTaskViewModel>();
        containerRegistry.Register<WorkViewModel>();
        containerRegistry.Register<ManageTasksView>();
        containerRegistry.RegisterSingleton<SettingsView>();
    }
}
