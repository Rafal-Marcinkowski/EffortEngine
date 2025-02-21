using SharedProject.Events;
using System.Windows;

namespace EffortEngine.MVVM.Views;

public partial class MainWindowView
{
    private readonly IEventAggregator eventAggregator;

    public MainWindowView(IEventAggregator eventAggregator)
    {
        InitializeComponent();

        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<BreakStartedEvent>().Subscribe(OnBreakStarted);
        this.eventAggregator.GetEvent<BreakEndedEvent>().Subscribe(OnBreakEnded);
    }

    private void OnBreakStarted()
    {
        MinimizeWindow();
    }

    private void OnBreakEnded()
    {
        FocusWindow();
    }

    private void MinimizeWindow()
    {
        var window = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);

        if (window != null)
        {
            window.WindowState = WindowState.Minimized;
        }
    }

    private void FocusWindow()
    {
        var window = Application.Current.Windows.OfType<Window>().FirstOrDefault();

        if (window != null)
        {
            window.WindowState = WindowState.Normal;
            window.Activate();
            window.Focus();
        }
    }
}