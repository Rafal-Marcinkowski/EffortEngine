using MahApps.Metro.IconPacks;
using SharedProject.Events;
using System.Windows.Media;
using System.Windows.Threading;

namespace EffortEngine.LocalLibrary;

public class PomodoroTimer : BindableBase, IDisposable
{
    private DispatcherTimer timer;
    private int timeRemaining;
    private int roundsCompleted = 0;
    private bool isBreak = false;
    private MediaPlayer soundPlayer = new();

    private async Task PlaySoundAsync(string filePath)
    {
        soundPlayer.Open(new Uri(filePath, UriKind.Absolute));
        soundPlayer.Play();
    }

    private static int activeWorkTime = 0;
    public static decimal ActiveWorkMinutes => Math.Round(activeWorkTime / 60m, 2);


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
        ResetCommand = new AsyncDelegateCommand(Reset);

        InitializeTimer();

        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<SessionFinishedEvent>().Subscribe(async () => await HandleSessionFinished());
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

    private async Task HandleSessionFinished()
    {
        await Reset();
        CurrentTaskText = string.Empty;
        RoundCounter = "Brak sesji";
    }

    private async Task Reset()
    {
        timer.Stop();
        IsRunning = false;
        StartPauseText = "Start";
        StartPauseIcon = PackIconMaterialKind.Play;
        timeRemaining = 25 * 60;
        roundsCompleted = 0;
        RoundCounter = $"Runda {roundsCompleted + 1}/4";
        isBreak = false;
        totalWork = 0;
        activeWorkTime = 0;
        TimerDisplay = $"00:00";
    }

    private async void OnTimerTick(object sender, EventArgs e)
    {
        if (timeRemaining > 0)
        {
            timeRemaining--;

            if (!isBreak && IsRunning)
            {
                totalWork++;
                activeWorkTime++;
            }

            TimerDisplay = $"{(isBreak ? "Przerwa" : "Praca")}: {TimeSpan.FromSeconds(timeRemaining):mm\\:ss}";
        }

        if (timeRemaining == 0)
        {
            isBreak = !isBreak;
            timeRemaining = isBreak ? 5 * 60 : 25 * 60;

            if (isBreak)
            {
                eventAggregator.GetEvent<BreakStartedEvent>().Publish();
                PlaySoundAsync("D:\\Microsoft Visual Studio 2022\\ImportantProjects\\EffortEngine\\EffortEngine\\LocalLibrary\\Miscellaneous\\breakstart.mp3");
            }

            else
            {
                eventAggregator.GetEvent<BreakEndedEvent>().Publish();
                roundsCompleted++;

                if (roundsCompleted < 4)
                {
                    PlaySoundAsync("D:\\Microsoft Visual Studio 2022\\ImportantProjects\\EffortEngine\\EffortEngine\\LocalLibrary\\Miscellaneous\\breakend.mp3");
                }

                RoundCounter = $"Runda {roundsCompleted + 1}/4";

                if (roundsCompleted >= 4)
                {
                    eventAggregator.GetEvent<SessionElapsedEvent>().Publish();
                    PlaySoundAsync("D:\\Microsoft Visual Studio 2022\\ImportantProjects\\EffortEngine\\EffortEngine\\LocalLibrary\\Miscellaneous\\sessioncompleted.mp3");
                    await Task.Delay(250);
                    await Reset();
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