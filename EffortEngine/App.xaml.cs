using DataAccess.Data;
using DataAccess.DBAccess;
using EffortEngine.LocalLibrary;
using EffortEngine.LocalLibrary.Services;
using EffortEngine.MVVM.ViewModels;
using EffortEngine.MVVM.Views;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Extensions.Configuration;
using Serilog;
using SharedProject;
using SharedProject.Services;
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
        regionManager.RequestNavigate("PomodoroBarRegion", nameof(PomodoroBarView));
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance<IConfiguration>(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build());

        containerRegistry.RegisterInstance<ILogger>(Log.Logger);

        containerRegistry.RegisterForNavigation<ManageTasksView>();
        containerRegistry.RegisterForNavigation<AddProgrammingTaskView>();
        containerRegistry.RegisterForNavigation<PomodoroBarView>();
        containerRegistry.RegisterForNavigation<TaskTableView>();
        containerRegistry.RegisterForNavigation<ProgramsTaskTableView, TaskTableViewModel>();
        containerRegistry.RegisterForNavigation<AllTasksTableView, TaskTableViewModel>();
        containerRegistry.RegisterForNavigation<AddGeneralTaskView>();
        containerRegistry.RegisterForNavigation<MainMenuView>();
        containerRegistry.RegisterForNavigation<AddTaskView>();
        containerRegistry.RegisterForNavigation<SettingsView>();

        containerRegistry.Register<MainMenuViewModel>();
        containerRegistry.Register<AddProgrammingTaskViewModel>();
        containerRegistry.Register<AddTaskViewModel>();
        containerRegistry.Register<AddGeneralTaskViewModel>();

        containerRegistry.RegisterSingleton<ConfigService>();
        containerRegistry.RegisterSingleton<IDialogCoordinator, DialogCoordinator>();
        containerRegistry.RegisterSingleton<ManageTasksViewModel>();
        containerRegistry.RegisterSingleton<TaskTableViewModel>();
        containerRegistry.RegisterSingleton<DataCoordinator>();
        containerRegistry.RegisterSingleton<ViewManager>();
        containerRegistry.RegisterSingleton<WorkManager>();
        containerRegistry.RegisterSingleton<PomodoroTimer>();
        containerRegistry.RegisterSingleton<TaskManager>();
        containerRegistry.RegisterSingleton<SessionManager>();
        containerRegistry.RegisterSingleton<ISQLDataAccess, SQLDataAccess>();
        containerRegistry.RegisterSingleton<IProgramData, ProgramData>();
        containerRegistry.RegisterSingleton<IPomodoroSessionData, PomodoroSessionData>();
        containerRegistry.RegisterSingleton<ITaskData, TaskData>();
        containerRegistry.RegisterSingleton<PomodoroBarViewModel>();
        containerRegistry.RegisterSingleton<SettingsViewModel>();
    }
}
