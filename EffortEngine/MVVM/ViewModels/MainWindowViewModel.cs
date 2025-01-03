namespace EffortEngine.MVVM.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private bool _isExpanded;
    private double _windowWidth;

    public MainWindowViewModel()
    {
        IsExpanded = false; // Start with collapsed state
        WindowWidth = 50; // Initial width for collapsed state
        ToggleWindowCommand = new DelegateCommand(ToggleWindow);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetProperty(ref _isExpanded, value);
    }

    public double WindowWidth
    {
        get => _windowWidth;
        set => SetProperty(ref _windowWidth, value);
    }

    public DelegateCommand ToggleWindowCommand { get; }

    private void ToggleWindow()
    {
        if (IsExpanded)
        {
            // Zwijanie
            IsExpanded = false;
            WindowWidth = 50;
        }
        else
        {
            // Rozwijanie
            IsExpanded = true;
            WindowWidth = 300; // Pełna szerokość
        }
    }
}