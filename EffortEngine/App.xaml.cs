using DataAccess.Data;
using DataAccess.DBAccess;
using EffortEngine.MVVM.ViewModels;
using EffortEngine.MVVM.Views;
using Microsoft.Extensions.Configuration;
using Serilog;
using SharedProject;
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

        LogManager.InitializeLogger();

        var regionManager = Container.Resolve<IRegionManager>();
        regionManager.RequestNavigate("MainRegion", nameof(MainMenuView));
        regionManager.RequestNavigate("InformationBarRegion", nameof(InformationBarView));
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build());

        containerRegistry.RegisterInstance<ILogger>(Log.Logger);

        containerRegistry.RegisterForNavigation<WorkView>();
        containerRegistry.RegisterForNavigation<ManageTasksView>();
        containerRegistry.RegisterForNavigation<AddProgrammingTaskView>();
        containerRegistry.RegisterForNavigation<AddModuleView>();
        containerRegistry.RegisterForNavigation<AddLifeTaskView>();
        containerRegistry.RegisterForNavigation<AddSystemTaskView>();
        containerRegistry.RegisterForNavigation<InformationBarView>();
        containerRegistry.RegisterForNavigation<AddBugView>();
        containerRegistry.RegisterForNavigation<AddStockMarketTaskView>();
        containerRegistry.RegisterForNavigation<AddFeatureView>();
        containerRegistry.RegisterForNavigation<AddProgramProjectView>();
        containerRegistry.RegisterForNavigation<MainMenuView>();
        containerRegistry.RegisterForNavigation<AddTaskView>();
        containerRegistry.RegisterForNavigation<SettingsView>();

        containerRegistry.Register<MainMenuViewModel>();
        containerRegistry.Register<AddProgrammingTaskViewModel>();
        containerRegistry.Register<AddLifeTaskViewModel>();
        containerRegistry.Register<AddSystemTaskViewModel>();
        containerRegistry.Register<AddBugViewModel>();
        containerRegistry.Register<AddStockMarketTaskViewModel>();
        containerRegistry.Register<AddFeatureViewModel>();
        containerRegistry.Register<AddProgramProjectViewModel>();
        containerRegistry.Register<AddModuleViewModel>();
        containerRegistry.Register<AddTaskViewModel>();
        containerRegistry.Register<WorkViewModel>();
        containerRegistry.Register<ManageTasksView>();
        containerRegistry.RegisterSingleton<ISQLDataAccess, SQLDataAccess>();
        containerRegistry.RegisterSingleton<IProgramData, ProgramData>();
        containerRegistry.RegisterSingleton<IPomodoroSessionData, PomodoroSessionData>();
        containerRegistry.RegisterSingleton<ITaskData, TaskData>();
        containerRegistry.RegisterSingleton<InformationBarViewModel>();
        containerRegistry.RegisterSingleton<SettingsView>();
    }
}
