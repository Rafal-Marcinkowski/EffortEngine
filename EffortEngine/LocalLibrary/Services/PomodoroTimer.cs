using MahApps.Metro.IconPacks;
using SharedProject.Events;
using SharedProject.Models;
using SharedProject.Services;
using System.IO;
using System.Windows.Media;
using System.Windows.Threading;

namespace EffortEngine.LocalLibrary.Services;

public class PomodoroTimer : BindableBase, IDisposable
{
    private readonly ConfigService _configService;
    private PomodoroConfig _config;
    private DispatcherTimer _timer;
    private int _timeRemaining;
    private int _roundsCompleted = 0;
    private bool _isBreak = false;
    private readonly MediaPlayer _soundPlayer = new();
    private static int _activeWorkTime = 0;
    private static int _totalWork = 0;
    private readonly IEventAggregator _eventAggregator;

    public PomodoroTimer(IEventAggregator eventAggregator, ConfigService configService)
    {
        _eventAggregator = eventAggregator;
        _configService = configService;
        _config = _configService.LoadConfig();

        StartPauseCommand = new AsyncDelegateCommand(OnStartPause);
        ResetCommand = new AsyncDelegateCommand(Reset);
        FinishSessionCommand = new AsyncDelegateCommand(FinishSession);
        StopCommand = new AsyncDelegateCommand(Stop);

        InitializeTimer();
        ResetTimerValues();

        _eventAggregator.GetEvent<SessionFinishedEvent>().Subscribe(async () => await Reset());
        _eventAggregator.GetEvent<ConfigUpdatedEvent>().Subscribe(async config => await UpdateConfig(config));
    }

    public async Task UpdateConfig(PomodoroConfig config)
    {
        if (SessionManager.IsSessionAlive) return;

        _config = config;
        ResetTimerValues();
    }

    private void InitializeTimer()
    {
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += OnTimerTick;
    }

    private void ResetTimerValues()
    {
        _timeRemaining = _config.WorkDurationMinutes * 60;
        TimerDisplay = $"Praca: {TimeSpan.FromMinutes(_config.WorkDurationMinutes):mm\\:ss}";
        RoundCounter = $"Brak sesji";
    }

    public IAsyncCommand StartPauseCommand { get; }
    public IAsyncCommand StopCommand { get; }
    public IAsyncCommand FinishSessionCommand { get; }
    public IAsyncCommand ResetCommand { get; }

    private string _currentTaskText = string.Empty;
    public string CurrentTaskText
    {
        get => _currentTaskText;
        set => SetProperty(ref _currentTaskText, value);
    }

    private string _timerDisplay = "00:00";
    public string TimerDisplay
    {
        get => _timerDisplay;
        set => SetProperty(ref _timerDisplay, value);
    }

    private string _roundCounter = "Brak sesji";
    public string RoundCounter
    {
        get => _roundCounter;
        set => SetProperty(ref _roundCounter, value);
    }

    private string _startPauseText = "Start";
    public string StartPauseText
    {
        get => _startPauseText;
        set => SetProperty(ref _startPauseText, value);
    }

    private PackIconMaterialKind _startPauseIcon = PackIconMaterialKind.Play;
    public PackIconMaterialKind StartPauseIcon
    {
        get => _startPauseIcon;
        set => SetProperty(ref _startPauseIcon, value);
    }

    private bool _isRunning = false;
    public bool IsRunning
    {
        get => _isRunning;
        set => SetProperty(ref _isRunning, value);
    }

    public static decimal ActiveWorkMinutes => Math.Round(_activeWorkTime / 60m, 2);
    public static decimal TotalWorkMinutes => Math.Round(_totalWork / 60m, 2);
    public static void ResetWorkTime() => _totalWork = 0;

    private async Task OnStartPause()
    {
        if (!string.IsNullOrEmpty(CurrentTaskText))
        {
            if (_timeRemaining == 0)
            {
                _timeRemaining = _config.WorkDurationMinutes * 60;
                _isBreak = false;
            }

            if (IsRunning)
            {
                _timer.Stop();
            }
            else
            {
                _timer.Start();
                RoundCounter = $"Runda {_roundsCompleted + 1}/{_config.RoundsCount}";
            }

            IsRunning = !IsRunning;
            StartPauseText = IsRunning ? "Pause" : "Start";
            StartPauseIcon = IsRunning ? PackIconMaterialKind.Pause : PackIconMaterialKind.Play;
        }
    }

    private async Task Stop()
    {
        _timer.Stop();
        IsRunning = false;
        StartPauseText = "Start";
        StartPauseIcon = PackIconMaterialKind.Play;
    }

    private async Task FinishSession()
    {
        await Reset();
    }

    private async Task Reset()
    {
        _timer.Stop();
        IsRunning = false;
        StartPauseText = "Start";
        StartPauseIcon = PackIconMaterialKind.Play;
        ResetTimerValues();
        _roundsCompleted = 0;
        _isBreak = false;
        _totalWork = 0;
        CurrentTaskText = string.Empty;
        RoundCounter = "Brak sesji";
        _activeWorkTime = 0;
    }

    private async void OnTimerTick(object sender, EventArgs e)
    {
        if (_timeRemaining > 0)
        {
            _timeRemaining--;

            if (!_isBreak && IsRunning)
            {
                _totalWork++;
                _activeWorkTime++;
            }

            TimerDisplay = $"{(_isBreak ? "Przerwa" : "Praca")}: {TimeSpan.FromSeconds(_timeRemaining):mm\\:ss}";
        }

        if (_timeRemaining == 0)
        {
            if (!_isBreak)
            {
                _roundsCompleted++;

                if (_roundsCompleted >= _config.RoundsCount)
                {
                    _eventAggregator.GetEvent<SessionElapsedEvent>().Publish();
                    await PlaySoundAsync("sessioncompleted.mp3");
                    await Task.Delay(250);
                    await FinishSession();
                    return;
                }

                _isBreak = true;
                _timeRemaining = _config.BreakDurationMinutes * 60;
                _eventAggregator.GetEvent<BreakStartedEvent>().Publish();
                await PlaySoundAsync("breakstart.mp3");
            }
            else
            {
                _isBreak = false;
                _timeRemaining = _config.WorkDurationMinutes * 60;
                _eventAggregator.GetEvent<BreakEndedEvent>().Publish();
                await PlaySoundAsync("breakend.mp3");
                RoundCounter = $"Runda {_roundsCompleted + 1}/{_config.RoundsCount}";
            }
        }
    }

    private async Task PlaySoundAsync(string fileName)
    {
        try
        {
            var soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\", "Sounds", fileName);
            if (File.Exists(soundPath))
            {
                _soundPlayer.Open(new Uri(soundPath));
                _soundPlayer.Play();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error playing sound: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _timer.Tick -= OnTimerTick;
        _timer.Stop();
        _soundPlayer.Close();
    }
}