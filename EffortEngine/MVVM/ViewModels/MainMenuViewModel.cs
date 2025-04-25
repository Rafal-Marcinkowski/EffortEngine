using EffortEngine.LocalLibrary.Services;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class MainMenuViewModel(ViewManager viewManager) : BindableBase
{
    public ICommand AddTaskCommand => new DelegateCommand(async () => await viewManager.NavigateToAddTask());

    public ICommand ManageTasksCommand => new DelegateCommand(async () => await viewManager.NavigateToManageTasks());
}
