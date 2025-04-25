using EffortEngine.MVVM.Views;

namespace EffortEngine.LocalLibrary.Services;

public class ViewManager(IRegionManager regionManager)
{
    private readonly IRegionManager regionManager = regionManager;

    public async Task NavigateToView(string regionName, string viewName)
    {
        var region = regionManager.Regions[regionName];
        if (region != null)
        {
            region.RemoveAll();
            regionManager.RequestNavigate(regionName, viewName);
        }
    }

    public async Task NavigateToMainMenu()
    {
        await NavigateToView("MainRegion", nameof(MainMenuView));
    }

    public async Task NavigateToAddTask()
    {
        await NavigateToView("MainRegion", nameof(AddTaskView));
    }

    public async Task NavigateToManageTasks()
    {
        await NavigateToView("MainRegion", nameof(ManageTasksView));
    }

    public async Task NavigateToSettings()
    {
        await NavigateToView("MainRegion", nameof(SettingsView));
    }

    public async Task NavigateToAddProgrammingTask()
    {
        await NavigateToView("MainRegion", nameof(AddProgrammingTaskView));
    }

    public async Task NavigateToAddGeneralTask()
    {
        await NavigateToView("MainRegion", nameof(AddGeneralTaskView));
    }
}
