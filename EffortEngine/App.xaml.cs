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

    }
}
