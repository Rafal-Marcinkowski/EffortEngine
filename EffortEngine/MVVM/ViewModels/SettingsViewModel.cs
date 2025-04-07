using MahApps.Metro.Controls.Dialogs;
using SharedProject.Models;
using SharedProject.Services;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class SettingsViewModel : BindableBase
{
    private readonly ConfigService _configService;
    private PomodoroConfig _currentConfig;
    private IDialogCoordinator _dialogCoordinator;

    public int WorkDurationMinutes
    {
        get => _currentConfig.WorkDurationMinutes;
        set
        {
            _currentConfig.WorkDurationMinutes = value;
            RaisePropertyChanged();
        }
    }

    public int BreakDurationMinutes
    {
        get => _currentConfig.BreakDurationMinutes;
        set
        {
            _currentConfig.BreakDurationMinutes = value;
            RaisePropertyChanged();
        }
    }

    public int RoundsCount
    {
        get => _currentConfig.RoundsCount;
        set
        {
            _currentConfig.RoundsCount = value;
            RaisePropertyChanged();
        }
    }

    public ICommand SaveCommand { get; }
    public ICommand ResetToDefaultsCommand { get; }

    public SettingsViewModel(ConfigService configService, IDialogCoordinator dialogCoordinator)
    {
        _configService = configService;
        _currentConfig = _configService.LoadConfig();
        _dialogCoordinator = dialogCoordinator;

        SaveCommand = new AsyncDelegateCommand(SaveSettings);
        ResetToDefaultsCommand = new AsyncDelegateCommand(ResetToDefaults);
    }

    private async Task SaveSettings()
    {
        _configService.SaveConfig(_currentConfig);
        await _dialogCoordinator.ShowMessageAsync(this, "Sukces", "Ustawienia zostały zapisane!");
    }

    private async Task ResetToDefaults()
    {
        _currentConfig = new PomodoroConfig();
        await _dialogCoordinator.ShowMessageAsync(this, "Przywrócono", "Domyślne ustawienia zostały przywrócone.");
    }
}