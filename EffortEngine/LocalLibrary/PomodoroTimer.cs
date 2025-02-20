using MahApps.Metro.IconPacks;
using SharedProject.Events;
using System.Windows.Threading;

namespace EffortEngine.LocalLibrary;

public class PomodoroTimer : BindableBase, IDisposable
{
    private DispatcherTimer timer;
    private int timeRemaining;
    private int roundsCompleted = 0;
    private bool isBreak = false;

    private static int totalWork = 0;

    public static decimal TotalWorkMinutes => Math.Round(totalWork / 60m, 2);

    public static void ResetWorkTime() => totalWork = 0;

    public IAsyncCommand StartPauseCommand { get; }
    public IAsyncCommand StopCommand { get; }
    public IAsyncCommand FinishSessionCommand { get; }
    public IAsyncCommand ResetCommand { get; }

    public PomodoroTimer(IEventAggregator eventAggregator)
    {
        StartPauseCommand = new AsyncDelegateCommand(OnStartPause);
        StopCommand = new AsyncDelegateCommand(OnStop);
        ResetCommand = new AsyncDelegateCommand(OnReset);


        InitializeTimer();

        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<SessionFinishedEvent>().Subscribe(async () => await OnReset());
    }

    private string currentTaskText = string.Empty;
    public string CurrentTaskText
    {
        get => currentTaskText;
        set => SetProperty(ref currentTaskText, value);
    }

    private string timerDisplay = "00:00";
    public string TimerDisplay
    {
        get => timerDisplay;
        set => SetProperty(ref timerDisplay, value);
    }

    private string roundCounter = "Brak sesji";
    public string RoundCounter
    {
        get => roundCounter;
        set => SetProperty(ref roundCounter, value);
    }

    private string startPauseText = "Start";
    public string StartPauseText
    {
        get => startPauseText;
        set => SetProperty(ref startPauseText, value);
    }

    private PackIconMaterialKind startPauseIcon;
    public PackIconMaterialKind StartPauseIcon
    {
        get => startPauseIcon;
        set => SetProperty(ref startPauseIcon, value);
    }

    private bool isRunning = false;
    private readonly IEventAggregator eventAggregator;

    public bool IsRunning
    {
        get => isRunning;
        set => SetProperty(ref isRunning, value);
    }

    private void InitializeTimer()
    {
        timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        StartPauseText = IsRunning ? "Pause" : "Start";
        StartPauseIcon = IsRunning ? PackIconMaterialKind.Pause : PackIconMaterialKind.Play;

        timer.Tick += OnTimerTick;
    }

    private async Task OnStartPause()
    {
        if (!String.IsNullOrEmpty(CurrentTaskText))
        {
            if (timeRemaining == 0)
            {
                timeRemaining = 25 * 60;
                isBreak = false;
            }

            if (IsRunning)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
                RoundCounter = $"Runda {roundsCompleted + 1}/4";
            }

            IsRunning = !IsRunning;
            StartPauseText = IsRunning ? "Pause" : "Start";
            StartPauseIcon = IsRunning ? PackIconMaterialKind.Pause : PackIconMaterialKind.Play;
        }
    }

    private async Task OnStop()
    {
        timer.Stop();
        IsRunning = false;
        StartPauseText = "Start";
        StartPauseIcon = PackIconMaterialKind.Play;
    }

    private async Task OnReset()
    {
        timer.Stop();
        ResetTimer();
        IsRunning = false;
        StartPauseText = "Start";
        StartPauseIcon = PackIconMaterialKind.Play;
    }

    private async Task ResetTimer()
    {
        timeRemaining = 25 * 60;
        roundsCompleted = 0;
        isBreak = false;
        totalWork = 0;
        TimerDisplay = $"{(isBreak ? "Przerwa" : "Praca")}: {TimeSpan.FromSeconds(timeRemaining):mm\\:ss}";
    }

    private async void OnTimerTick(object sender, EventArgs e)
    {
        if (timeRemaining > 0)
        {
            timeRemaining--;

            if (!isBreak && IsRunning)
            {
                totalWork++;
            }

            TimerDisplay = $"{(isBreak ? "Przerwa" : "Praca")}: {TimeSpan.FromSeconds(timeRemaining):mm\\:ss}";
        }

        if (timeRemaining == 0)
        {
            isBreak = !isBreak;
            timeRemaining = isBreak ? 5 * 60 : 25 * 60;

            if (!isBreak)
            {
                roundsCompleted++;
                RoundCounter = $"Runda {roundsCompleted + 1}/4";

                if (roundsCompleted >= 4)
                {
                    await OnStop();
                    await ResetTimer();
                    return;
                }
            }
        }
    }

    void IDisposable.Dispose()
    {
        timer.Tick -= OnTimerTick;
        timer.Stop();
    }
}
