using EffortEngine.LocalLibrary.Services;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class AddTaskViewModel(ViewManager viewManager) : BindableBase
{
    public ICommand AddProgrammingTaskCommand => new DelegateCommand(async () => await viewManager.NavigateToAddProgrammingTask());

    public ICommand AddGeneralTaskCommand => new DelegateCommand(async () => await viewManager.NavigateToAddGeneralTask());
}
