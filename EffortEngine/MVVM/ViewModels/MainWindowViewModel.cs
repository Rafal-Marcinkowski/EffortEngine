using EffortEngine.LocalLibrary.Services;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class MainWindowViewModel(ViewManager viewManager) : BindableBase
{
    public ICommand AddTaskCommand => new DelegateCommand(async () => await viewManager.NavigateToAddTask());

    public ICommand ManageTasksCommand => new DelegateCommand(async () => await viewManager.NavigateToManageTasks());

    public ICommand MainMenuCommand => new DelegateCommand(async () => await viewManager.NavigateToMainMenu());

    public ICommand SettingsCommand => new DelegateCommand(async () => await viewManager.NavigateToSettings());
}