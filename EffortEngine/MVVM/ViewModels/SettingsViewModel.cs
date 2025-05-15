using MahApps.Metro.Controls.Dialogs;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Services;
using System.Windows.Input;

namespace EffortEngine.MVVM.ViewModels;

public class SettingsViewModel : BindableBase
{
    public SettingsViewModel(ConfigService configService, IDialogCoordinator dialogCoordinator, IEventAggregator eventAggregator)
    {
        _configService = configService;
        CurrentConfig = _configService.LoadConfig();
        _dialogCoordinator = dialogCoordinator;
        this.eventAggregator = eventAggregator;
        SaveCommand = new AsyncDelegateCommand(SaveSettings);
        ResetToDefaultsCommand = new AsyncDelegateCommand(ResetToDefaults);
    }

    private readonly ConfigService _configService;
    public PomodoroConfig CurrentConfig { get; set; }
    private readonly IDialogCoordinator _dialogCoordinator;
    private readonly IEventAggregator eventAggregator;

    public ICommand SaveCommand { get; }
    public ICommand ResetToDefaultsCommand { get; }

    private async Task SaveSettings()
    {
        _configService.SaveConfig(CurrentConfig);
        RaisePropertyChanged(nameof(CurrentConfig));
        this.eventAggregator.GetEvent<ConfigUpdatedEvent>().Publish(CurrentConfig);
        await _dialogCoordinator.ShowMessageAsync(this, "Sukces", "Ustawienia zostały zapisane!");
    }

    private async Task ResetToDefaults()
    {
        CurrentConfig = new PomodoroConfig();
        _configService.SaveConfig(CurrentConfig);
        RaisePropertyChanged(nameof(CurrentConfig));
        this.eventAggregator.GetEvent<ConfigUpdatedEvent>().Publish(CurrentConfig);
        await _dialogCoordinator.ShowMessageAsync(this, "Przywrócono", "Domyślne ustawienia zostały przywrócone.");
    }
}